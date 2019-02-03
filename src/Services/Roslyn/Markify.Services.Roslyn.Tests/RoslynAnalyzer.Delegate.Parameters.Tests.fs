namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerDelegateParametersTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Swensen.Unquote
    open Xunit

    [<Theory>]
    [<ProjectData("Delegates", "WithoutParameters")>]
    let ``Analyze should return no parameters when delegate has none`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getParameters

        <@ actual |> Seq.isEmpty @>

    [<Theory>]
    [<ProjectData("Delegates", "WithOneParameter")>]
    [<ProjectData("Delegates", "WithMultipleParameters")>]
    let ``Analyze should return parameters when delegate has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getParameters

        <@ actual |> (Seq.isEmpty >> not) @>
    
    [<Theory>]
    [<ProjectData("Delegates", "WithOneParameter", "foo")>]
    [<ProjectData("Delegates", "WithMultipleParameters", "bar")>]
    let ``Analyze should return expected delegate parameter name`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getParameters

        test <@ actual |> Seq.exists (fun c -> c.Name = parameter) @>
    
    [<Theory>]
    [<ProjectData("Delegates", "WithMultipleParameters", "foo", "Int32")>]
    [<ProjectData("Delegates", "WithMultipleParameters", "bar", "Single")>]
    [<ProjectData("Delegates", "WithGenericParameters`1", "foo", "T")>]
    let ``Analyze should return expected delegate parameter type`` (name, parameter, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> TestHelper.getDelegateParameter parameter

        <@ actual.Type = expected @>
    
    [<Theory>]
    [<ProjectData("Delegates", "WithOneParameter", "foo")>]
    let ``Analyze should return no modifier when delegate parameter has none`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> TestHelper.getDelegateParameter parameter

        <@ actual.Modifier = None @>

    [<Theory>]
    [<ProjectData("Delegates", "WithParametersModifiers", "foo", "ref")>]
    [<ProjectData("Delegates", "WithParametersModifiers", "foo", "out")>]
    let ``Analyze should return modifiers when delegate parameter has some`` (name, parameter, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelperOld.getMemberModifiers project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> TestHelper.getDelegateParameter parameter
        
        <@ actual.Modifier = Some expected @>
    
    [<Theory>]
    [<ProjectData("Delegates", "WithOneParameter", "foo")>]
    let ``Analyze should return no default value when delegate parameter has none`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> TestHelper.getDelegateParameter parameter

        <@ actual.DefaultValue = None @>

    [<Theory>]
    [<ProjectData("Delegates", "WithDefaultParameters", "foo", "1")>]
    let ``Analyze should return default value when delegate parameter has some`` (name, parameter, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> TestHelper.getDelegateParameter parameter
        
        <@ actual.DefaultValue = Some expected @>