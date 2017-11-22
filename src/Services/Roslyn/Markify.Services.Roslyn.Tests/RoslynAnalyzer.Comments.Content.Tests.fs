namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerCommentsContentTests =
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    type ClassDataAttribute = Markify.Services.Roslyn.Tests.ClassDataAttribute

    let getComment findComment = function
        | Class x | Struct x | Interface x -> findComment x.Comments
        | Enum x -> findComment x.Comments
        | Delegate x -> findComment x.Comments
       
    [<Theory>]
    [<ProjectData("Comments", "EnumWithSimpleComments")>]
    [<ProjectData("Comments", "ClassWithSimpleComments`2")>]
    [<ProjectData("Comments", "StructWithSimpleComments`2")>]
    [<ProjectData("Comments", "InterfaceWithSimpleComments`2")>]
    [<ProjectData("Comments", "DelegateWithSimpleComments`2")>]
    let ``Analyze should return a single content when comment has only one type of content`` (name, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)

        test <@ actual.Value.Content |> Seq.length = 1 @>
    
    [<Theory>]
    [<ProjectData("Comments", "EnumWithComplexComments")>]
    [<ProjectData("Comments", "ClassWithComplexComments`2")>]
    [<ProjectData("Comments", "StructWithComplexComments`2")>]
    [<ProjectData("Comments", "InterfaceWithComplexComments`2")>]
    [<ProjectData("Comments", "DelegateWithComplexComments`2")>]
    let ``Analyze should return multiple content when comment has a mixed type of content`` (name, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)

        test <@ actual.Value.Content |> Seq.length > 1 @>

    [<Theory>]
    [<ProjectData("Comments", "EnumWithSimpleComments")>]
    [<ProjectData("Comments", "ClassWithSimpleComments`2")>]
    [<ProjectData("Comments", "StructWithSimpleComments`2")>]
    [<ProjectData("Comments", "InterfaceWithSimpleComments`2")>]
    [<ProjectData("Comments", "DelegateWithSimpleComments`2")>]
    let ``Analyze should return comment with text contents when comment has texts`` (name, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
        
        test <@ actual.Value.Content |> Seq.filter (function | Text x -> true | _ -> false)
                                     |> Seq.length > 0 @> 

    [<Theory>]
    [<ProjectData("Comments", "EnumWithComplexComments")>]
    [<ProjectData("Comments", "ClassWithComplexComments`2")>]
    [<ProjectData("Comments", "StructWithComplexComments`2")>]
    [<ProjectData("Comments", "InterfaceWithComplexComments`2")>]
    [<ProjectData("Comments", "DelegateWithComplexComments`2")>]
    let ``Analyze should return comment with block contents when comment has nested tag`` (name, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)

        test <@ actual.Value.Content |> Seq.filter (function | Block x -> true | _ -> false)
                                     |> Seq.length > 0 @> 
    
    [<Theory>]
    [<ProjectData("Comments", "EnumWithComplexComments")>]
    [<ProjectData("Comments", "ClassWithComplexComments`2")>]
    [<ProjectData("Comments", "StructWithComplexComments`2")>]
    [<ProjectData("Comments", "InterfaceWithComplexComments`2")>]
    [<ProjectData("Comments", "DelegateWithComplexComments`2")>]
    let ``Analyze should return all nested tag when comment has multi level tag`` (name, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let comment =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Remarks)
        let actual =
            comment.Value.Content
            |> Seq.choose (function | Block x -> Some x | _ -> None)
            |> Seq.head
            |> fun c -> c.Content
            |> Seq.choose (function | Block x -> Some x | _ -> None)

        test <@ actual |> Seq.length > 0 @>