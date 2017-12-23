namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerNamespacesTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Empty")>]
    let ``Analyze should return no namespace when project has none`` (sut : RoslynAnalyzer, project) =
        let actual = (sut :> IProjectAnalyzer).Analyze project

        test <@ actual.Namespaces |> Seq.isEmpty @>

    [<Theory>]
    [<ProjectData("Organization")>]
    let ``Analyze should return namespaces when type has some`` (sut : RoslynAnalyzer, project) =
        let actual = (sut :> IProjectAnalyzer).Analyze project

        test <@ actual.Namespaces |> (Seq.isEmpty >> not) @>

    [<Theory>]
    [<ProjectData("Organization", "Class")>]
    let ``Analyze should return namespace name`` (expected, sut : RoslynAnalyzer, project) =
        let actual = (sut :> IProjectAnalyzer).Analyze project

        test <@ actual.Namespaces |> Seq.tryFind (fun c -> c.Name = expected)
                                  |> Option.isSome @>