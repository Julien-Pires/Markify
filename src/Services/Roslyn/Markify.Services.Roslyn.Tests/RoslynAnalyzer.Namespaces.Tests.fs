namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerNamespacesTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("TypesInformations/EmptySourceSamples", ProjectLanguage.CSharp, 0)>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.CSharp, 2)>]
    [<MultiProjectData("TypesInformations/EmptySourceSamples", ProjectLanguage.VisualBasic, 0)>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.VisualBasic, 2)>]
    let ``Analyze should return expected namespaces count`` (expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count = 
                    library.Namespaces
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace")>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace.BarNamespace")>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace")>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace.BarNamespace")>]
    let ``Analyze should return correct namespace name`` (expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                library.Namespaces::acc) []

        test <@ actual |> List.forall (fun c -> c |> Seq.exists (fun d -> d.Name = expected)) @>