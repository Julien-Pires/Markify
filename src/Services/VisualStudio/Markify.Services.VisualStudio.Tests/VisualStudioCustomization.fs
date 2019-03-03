namespace Markify.Services.VisualStudio.Tests

open System.Collections
open EnvDTE
open EnvDTE80
open Foq

type TreeItem =
    | Leaf of string
    | Node of string * TreeItem list

module Tree =
    let createTree depth nodeCount nodeName leafCount leafName =
        let rec loop acc currentDepth parent =
            let leafs =
                List.init leafCount (fun c -> sprintf "%s/%s" parent (leafName c))
                |> List.map Leaf
            match currentDepth with
            | 0 -> leafs |> List.append acc
            | _ ->
                let nodes =
                    List.init nodeCount (fun c -> sprintf "%s/%s" parent (nodeName c currentDepth))
                    |> List.map (fun c -> (c, loop [] (currentDepth - 1) c))
                    |> List.map Node
                nodes |> List.append leafs |> List.append acc

        let rootName = nodeName 0 0
        Node (rootName, loop [] depth rootName)

    let rec cata fLeaf fNode item =
        let recurse = cata fLeaf fNode
        match item with
        | Leaf name -> fLeaf name
        | Node (name, subItems) -> fNode name (subItems |> List.map recurse)

type ProjectConfiguration = {
    Folders : int 
    Files : int 
    Depth : int
    Language : string 
    Extension : string }

module ProjectBuilder =
    let private createContent rootPath (configuration : ProjectConfiguration) =
        let folderName index depth =
            sprintf "Folder_%i_%i" index depth

        let fileName index =
            sprintf "File_%i" index

        let file name =
            Mock<ProjectItem>.With(fun c ->
            <@ c.Kind --> Constants.vsProjectItemKindPhysicalFile
               c.get_FileNames(any()) --> sprintf "%s/%s.%s" rootPath name configuration.Extension @>)
        let folder name items =
            let files =
                Mock<ProjectItems>()
                    .Setup(fun c -> <@ c.GetEnumerator() @>).Returns(fun () -> (items :> IEnumerable).GetEnumerator())
                    .Create()
            Mock<ProjectItem>.With (fun c ->
                <@ c.Kind --> Constants.vsProjectItemKindPhysicalFolder
                   c.get_FileNames(any()) --> sprintf "%s/%s" rootPath name
                   c.ProjectItems --> files @>)

        Tree.createTree configuration.Depth configuration.Folders folderName configuration.Files fileName
        |> Tree.cata file folder

    let create rootPath name configuration =
        let language = Mock<CodeModel>.With (fun c -> <@ c.Language --> configuration.Language @>)
        let content =
            [createContent rootPath configuration]
            |> fun c -> 
                Mock<ProjectItems>()
                    .Setup(fun d -> <@ d.GetEnumerator() @>).Returns(fun () -> (c :> IEnumerable).GetEnumerator())
                    .Create()
        Mock<Project>.With (fun c ->
            <@ c.Name --> name
               c.FullName --> sprintf "%s/%s" rootPath name
               c.ProjectItems --> content
               c.CodeModel --> language @>)

type SolutionConfiguration = {
    RootPath : string
    Folders : int
    Projects : int
    Depth : int
    Project : ProjectConfiguration }

module SolutionBuilder =
    let private folderName index depth =
        sprintf "Folder_%i_%i" index depth

    let private projectName index = 
        sprintf "Project_%i" index

    let private createContent configuration =
        let project name =
            ProjectBuilder.create configuration.RootPath name configuration.Project
        let folder name items =
            let projects =
                items
                |> List.map (fun c -> Mock<ProjectItem>.With (fun d -> <@ d.SubProject --> c @>))
                |> fun c -> 
                    Mock<ProjectItems>()
                        .Setup(fun d -> <@ d.GetEnumerator() @>).Returns(fun () -> (c :> IEnumerable).GetEnumerator())
                        .Create()
            Mock<Project>.With (fun c ->
                <@ c.Name --> sprintf "%s.%sproj" name configuration.Project.Extension
                   c.Kind --> ProjectKinds.vsProjectKindSolutionFolder
                   c.ProjectItems --> projects @>)
        
        Tree.createTree configuration.Depth configuration.Folders folderName configuration.Projects projectName
        |> Tree.cata project folder

    let create configuration =
        let content = createContent configuration
        let currentProject = 
            content.ProjectItems
            |> Seq.cast<ProjectItem>
            |> Seq.tryFind (fun c -> c.Kind <> ProjectKinds.vsProjectKindSolutionFolder)
        let projects = 
            Mock<Projects>()
                .Setup(fun c -> <@ c.GetEnumerator() @>).Returns(fun () -> ([content] :> IEnumerable).GetEnumerator())
                .Setup(fun c -> <@ c.Item(any()) @>).Returns(match currentProject with | Some x -> x.SubProject | None -> null)
                .Create()
        Mock<Solution>.With (fun c -> 
            <@ c.FileName --> "Solution.sln"
               c.FullName --> sprintf "%s/Solution.sln" configuration.RootPath
               c.Projects --> projects @>)

type VisualStudioConfiguration = {
    Solution : SolutionConfiguration option }

module VisualStudioBuilder =
    let create configuration =
        let (solution, activeProject) =
            match configuration.Solution with
            | Some x -> 
                let solution = SolutionBuilder.create x
                (solution, match x.Projects with | 0 -> [| |] | _ -> [| solution.Projects.Item(0) |])
            | None -> (null, [| |])
        Mock<DTE2>.With (fun c -> 
            <@  c.Solution --> solution
                c.ActiveSolutionProjects --> (activeProject :> obj) @>)