namespace Markify.Roslyn.Tests

module Roslyn_Processor_Process_Namespaces_Tests =
    open Attributes
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Core.Analyzers

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("EmptySourceProject", ProjectLanguage.CSharp, 0)>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, 2)>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, 1)>]
    [<ProjectData("EmptySourceProject", ProjectLanguage.VisualBasic, 0)>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, 2)>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, 1)>]
    let ``Proces projects should return expected namespaces count`` (expected, sut: RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project

        test <@ Seq.length library.Namespaces = expected @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "FooSpace")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "FooSpace.InnerSpace")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "FooSpace")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "FooSpace.InnerSpace")>]
    let ``Process projects should return correct namespace name`` (expected, sut: RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let nspace =
            library.Namespaces
            |> Seq.tryFind (fun c -> c.Name = expected)

        test <@ nspace.IsSome @>