namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Empty")>]
    let ``Analyze should return no type defintion when project has no type`` (sut : RoslynAnalyzer, project) =
        let actual = (sut :> IProjectAnalyzer).Analyze project

        test <@ actual.Types |> Seq.isEmpty @>
    
    [<Theory>]
    [<ProjectData("Organization")>]
    let ``Analyze should return type definition when project has some`` (sut : RoslynAnalyzer, project) =
        let actual = (sut :> IProjectAnalyzer).Analyze project

        test <@ actual.Types |> Seq.isEmpty |> not @>

    [<Theory>]
    [<ProjectData("Duplicates")>]
    let ``Analyze should return no duplicate type definition`` (sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            library.Types
            |> Seq.map (fun c -> getFullname c.Identity)
            |> Seq.groupBy id

        test <@ actual |> Seq.map (fun c -> snd c |> Seq.length)
                       |> Seq.forall ((=) 1) @>

    [<Theory>]
    [<ProjectData("Organization", "FooType")>]
    [<ProjectData("Organization", "NestedType")>]
    [<ProjectData("Organization", "DeeperNestedType")>]
    let ``Analyze should return type definition with correct name`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = TestHelper.getDefinitions name library

        test <@ (actual |> Seq.length) > 0 @>
    
    [<Theory>]
    [<ProjectData("Organization", "FooType")>]
    [<ProjectData("Organization", "ParentType.NestedType")>]
    [<ProjectData("Organization", "ParentType.AnotherNestedType.DeeperNestedType")>]
    let ``Analyze should return type definition with correct fullname`` (name, sut : RoslynAnalyzer, project) =
        let fullnames = NamespaceHelper.AllTypes |> Seq.map (fun c -> sprintf "%s.%s" c name)
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = fullnames |> Seq.map (fun c -> TestHelper.getDefinitionByFullname c library)

        test <@ (actual |> Seq.length) = (fullnames |> Seq.length) @>