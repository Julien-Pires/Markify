namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerCommentsParametersTests =
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    let getComment findComment = function
        | Class x | Struct x | Interface x -> findComment x.Comments
        | Enum x -> findComment x.Comments
        | Delegate x -> findComment x.Comments

    [<Theory>]
    [<ProjectData("Comments", "WithNoParameter")>]
    let ``Analyze should return no comment parameters when comment has none``(name, sut : RoslynAnalyzer, project : ProjectInfo) = 
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
        
        test <@ actual.Value.Parameter |> Seq.length = 0 @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithOneParameter", 1)>]
    [<ProjectData("Comments", "WithMultipleParameter", 3)>]
    let ``Analyze should return comment parameters when comment has some``(name, expected, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)

        test <@ actual.Value.Parameter |> Seq.length = expected @>

    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value")>]
    let ``Analyze should return expected comment parameter name when comment has some``(name, expected, sut : RoslynAnalyzer, project : ProjectInfo) = 
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
            |> fun c -> c.Value.Parameter
            |> Seq.tryFind (fun c -> c.Name = expected) 
        
        test <@ actual |> Option.isSome @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "data")>]
    let ``Analyze should return no comment parameter value when parameter has none``(name, parameter, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
            |> fun c -> c.Value.Parameter
            |> Seq.find (fun c -> c.Name = parameter)

        test <@ actual.Value |> Option.isNone @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value")>]
    let ``Analyze should return comment parameter value when parameter has some``(name, parameter, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
            |> fun c -> c.Value.Parameter
            |> Seq.find (fun c -> c.Name = parameter)

        test <@ actual.Value |> Option.isSome @>

    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name", "foo")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value", "bar")>]
    let ``Analyze should return expected comment parameter value when parameter has some``(name, parameter, expected, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment (fun c -> c.Summary)
            |> fun c -> c.Value.Parameter
            |> Seq.find (fun c -> c.Name = parameter)

        test <@ actual.Value.Value = expected @>