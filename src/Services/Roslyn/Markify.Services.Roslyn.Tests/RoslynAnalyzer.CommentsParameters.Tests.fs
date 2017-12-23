namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerCommentsParametersTests =
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Comments", "WithNoParameter")>]
    let ``Analyze should return no comment parameters when comment has none``(name, sut : RoslynAnalyzer, project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getComment "summary"
        
        test <@ actual.Value.Parameter |> Seq.isEmpty @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithOneParameter", 1)>]
    [<ProjectData("Comments", "WithMultipleParameter", 3)>]
    let ``Analyze should return comment parameters when comment has some``(name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getComment "summary"

        test <@ actual.Value.Parameter |> Seq.length = expected @>

    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value")>]
    let ``Analyze should return expected comment parameter name when comment has some``(name, expected, sut : RoslynAnalyzer, project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getComment "summary"
            |> fun c -> c.Value.Parameter
            |> Seq.tryFind (fun c -> c.Name = expected) 
        
        test <@ actual.IsSome @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "data")>]
    let ``Analyze should return no comment parameter value when parameter has none``(name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> TestHelper.getCommentParameter "summary" parameter

        test <@ actual.Value = None @>
    
    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value")>]
    let ``Analyze should return comment parameter value when parameter has some``(name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> TestHelper.getCommentParameter "summary" parameter

        test <@ actual.Value.IsSome @>

    [<Theory>]
    [<ProjectData("Comments", "WithMultipleParameter", "name", "foo")>]
    [<ProjectData("Comments", "WithMultipleParameter", "value", "bar")>]
    let ``Analyze should return expected comment parameter value when parameter has some``(name, parameter, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> TestHelper.getCommentParameter "summary" parameter

        test <@ actual.Value.Value = expected @>