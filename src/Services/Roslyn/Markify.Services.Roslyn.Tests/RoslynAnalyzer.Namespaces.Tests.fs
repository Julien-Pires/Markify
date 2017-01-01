namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerNamespacesTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("EmptySourceSamples", ProjectLanguage.CSharp, 0)>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, 2)>]
    [<MultiProjectData("EmptySourceSamples", ProjectLanguage.VisualBasic, 0)>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, 2)>]
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
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace.BarNamespace")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace.BarNamespace")>]
    let ``Analyze should return correct namespace name`` (expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                library.Namespaces::acc) []

        test <@ actual |> List.forall (fun c -> c |> Seq.exists (fun d -> d.Name = expected)) @>