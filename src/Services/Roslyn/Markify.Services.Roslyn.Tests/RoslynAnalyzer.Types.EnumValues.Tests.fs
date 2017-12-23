namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesEnumValuesTests =
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote
    
    [<Theory>]
    [<ProjectData("Enum", "WithNoValues")>]
    let ``Analyze should return no values when enum has none`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getEnumValues
        
        test <@ actual |> Seq.isEmpty @>

    [<Theory>]
    [<ProjectData("Enum", "WithValues")>]
    let ``Analyze should return values when enum has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getEnumValues
        
        test <@ actual |> Seq.length > 0 @>

    [<Theory>]
    [<ProjectData("Enum", "WithValues", "Foo")>]
    [<ProjectData("Enum", "WithValues", "FooBar")>]
    let ``Analyze should return enum value name`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library
            |> DefinitionsHelper.getEnumValues
        
        test <@ actual |> Seq.exists (fun c -> c.Name = expected) @>
    
    [<Theory>]
    [<ProjectData("Enum", "WithValues", "BarFoo")>]
    let ``Analyze should return no underlying value when enum value has none`` (name, value, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getEnumValues
            |> Seq.find (fun c -> c.Name = value)
        
        test <@ actual.Value = None @>

    [<Theory>]
    [<ProjectData("Enum", "WithValues", "Foo", "0")>]
    let ``Analyze should return correct enum underlying value`` (name, value, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getEnumValues
            |> Seq.find (fun c -> c.Name = value)
        
        test <@ actual.Value = Some expected @>