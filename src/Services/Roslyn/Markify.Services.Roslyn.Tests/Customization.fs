namespace Markify.Services.Roslyn.Tests

open System
open System.IO
open System.Reflection
open System.Xml.Serialization
open Markify.Services.Roslyn.Fixtures
open Markify.Domain.Ide;
open Ploeh.AutoFixture

type ProjectInfo = {
    Project : Project
    Count : int }

type ProjectContextCustomization (file, language) =
    let file = file

    let extension =
        match language with
        | ProjectLanguage.CSharp -> "cs"
        | ProjectLanguage.VisualBasic -> "vb"
        | _ -> ""

    let projectExtension =
        match language with
        | ProjectLanguage.CSharp -> "csproj"
        | ProjectLanguage.VisualBasic -> "vbproj"
        | _ -> ""

    let (|IsValidSolution|_|) (path : string) =
        match path.Split([|'/'|], StringSplitOptions.RemoveEmptyEntries) with
        | [|x; y|] -> Some (x, y)
        | _ -> None

    let getFullPath path =
        let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
        let cleanPath = Uri.UnescapeDataString (basePath.Path)
        Path.Combine (Path.GetDirectoryName (cleanPath), path)

    let createProject (content : ProjectContent) = 
        let project = {   
            Name = "Test"
            Path = Uri(sprintf "c:/Test/Test.%s" projectExtension)
            Language = language
            Files =
                content.Files
                |> Seq.map (fun c -> 
                    let fullpath = getFullPath <| sprintf "Projects/%s" c
                    match language with                
                    | ProjectLanguage.Unsupported -> fullpath
                    | _ -> sprintf "%s.%s" fullpath extension)
                |> Seq.map Uri }
        {   Project = project
            Count = content.Count }

    member this.ReadXml<'T> (path : string) =
        let serializer = XmlSerializer(typeof<'T>)
        use stream = new StreamReader(path)
        serializer.Deserialize stream :?> 'T
        
    member this.GetProjects path =
        match path with
        | IsValidSolution x ->
            let file, solutionName = x
            (sprintf "Projects/%s.xml" file)
            |> getFullPath
            |> this.ReadXml
            |> fun (c : Solutions) -> c.AllSolution
            |> Seq.find (fun (c : SolutionContent) -> c.Name = solutionName)
            |> fun c -> c.Projects
        | _ -> raise (ArgumentException("Incorrect path format, must be of the form file/SolutionName", "path"))

    member this.BuildMultiProjects path =
        this.GetProjects path
        |> Array.map (createProject)

    interface ICustomization with
        member this.Customize (fixture : IFixture) =
            fixture.Inject (this.BuildMultiProjects file)