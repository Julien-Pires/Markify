module Roslyn_Processor_Process_Namespaces_Tests
    open Attributes
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Core.Analyzers

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectContextInlineAutoData("EmptySourceProject.xml", ProjectLanguage.CSharp, 0)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, 2)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, 1)>]
    [<ProjectContextInlineAutoData("EmptySourceProject.xml", ProjectLanguage.VisualBasic, 0)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, 2)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, 1)>]
    let ``Proces projects should return correct namespaces`` (expected, sut: RoslynAnalyzer, project: Project) =
        let library = (sut :> IProjectAnalyzer).Analyze project

        test <@ Seq.length library.Namespaces = expected @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "FooSpace")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "FooSpace.InnerSpace")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "FooSpace")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "FooSpace.InnerSpace")>]
    let ``Process projects with namespace should return correct name`` (expected, sut: RoslynAnalyzer, project: Project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let nspace =
            library.Namespaces
            |> Seq.tryFind (fun c -> c.Name = expected)

        test <@ nspace.IsSome @>