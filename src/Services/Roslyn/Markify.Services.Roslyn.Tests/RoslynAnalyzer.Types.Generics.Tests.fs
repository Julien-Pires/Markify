namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesGenericsTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Generics", "NoGenericType")>]
    let ``Analyze should return no generic parameters when type has none`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = TestHelper.getDefinitions name library

        test <@ actual |> Seq.forall (fun c -> c.Identity.Parameters |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", 1)>]
    [<ProjectData("Properties", "MultipleGenericType`2", 2)>]
    let ``Analyze should return generic parameters when type has some`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = TestHelper.getDefinitions name library

        test <@ actual |> Seq.map (fun c -> c.Identity.Parameters)
                       |> Seq.forall (fun c -> c |> Seq.length = expected) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", "T")>]
    [<ProjectData("Properties", "MultipleGenericType`2", "T")>]
    [<ProjectData("Properties", "MultipleGenericType`2", "Y")>]
    let ``Analyze should return generic parameter name when type has some`` (name, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map (fun c -> c.Identity.Parameters)

        test <@ actual |> Seq.forall (fun c -> c |> Seq.exists (fun d -> d.Name = expected)) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", "T")>]
    let ``Analyze should return no modifier when generic parameter has none`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getGenericParameter parameter

        test <@ actual |> Seq.forall (fun c -> c.Modifier.IsNone) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", "in")>]
    [<ProjectData("Properties", "SingleGenericType`1", "out")>]
    let ``Analyze should return correct parameter modifier`` (name, parameter, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getGenericParameter parameter

        test <@ actual |> Seq.forall (fun c -> c.Modifier = Some expected) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", "T")>]
    let ``Analyze should return no constraints when generic parameter has none`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getGenericParameter parameter

        test <@ actual |> Seq.forall (fun c -> c.Constraints |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Properties", "SingleGenericType`1", "T")>]
    [<ProjectData("Properties", "MultipleGenericType`2", "T")>]
    [<ProjectData("Properties", "MultipleGenericType`2", "Y")>]
    let ``Analyze should return constraints when generic parameter has some`` (name, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getGenericParameter parameter

        test <@ actual |> Seq.forall (fun c -> c.Constraints |> Seq.length > 0) @>

    [<Theory>]
    [<ProjectData("Properties", "MultipleGenericType`2", "Y", "struct")>]
    [<SingleLanguageProjectData("Properties", ProjectLanguage.CSharp, "MultipleGenericType`2", "Y", "class;IList<string>")>]
    [<SingleLanguageProjectData("Properties", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "Y", "Class;IList(Of String)")>]
    let ``Analyze should return correct constraint name when parameter has some`` (name, parameter, constraints : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiers constraints project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getGenericParameter parameter
            |> Seq.map (fun c -> Set c.Constraints)

        test <@ actual |> Seq.forall ((Set.isSubset) expected) @>