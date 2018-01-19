namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesPartialMergingTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Compiler
    open Swensen.Unquote
    open Xunit

    [<Theory>]
    [<ProjectData("PartialType", "FooType", 4)>]
    let ``Analyze should gather methods from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getMethods

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>
        
    [<Theory>]
    [<ProjectData("PartialType", "FooType", 2)>]
    let ``Analyze should gather properties from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getProperties

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>

    [<Theory>]
    [<ProjectData("PartialType", "Class.FooType", 2)>]
    [<ProjectData("PartialType", "Struct.FooType", 2)>]
    let ``Analyze should gather fields from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getFields

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>

    [<Theory>]
    [<ProjectData("PartialType", "FooType", 2)>]
    let ``Analyze should gather events from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getEvents

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>

    [<Theory>]
    [<ProjectData("PartialType", "Class.FooType", 2)>]
    let ``Analyze should gather modifiers from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> Seq.map DefinitionsHelper.getModifiers

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>

    [<Theory>]
    [<ProjectData("PartialType", "FooType", 2)>]
    let ``Analyze should gather inherited types from partial type`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getBaseTypes

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = expected) @>