namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_Namespaces_Tests =
    [<Tests>]
    let noNamespacesTests =
        let contents = [
            (ProjectLanguage.CSharp, [""])
            (ProjectLanguage.VisualBasic, [""])
        ]
        testList "Analyze/Namespace" [
            yield! testRepeat (withProjects contents)
                "should return no namespace when project has none"
                (fun sut project () -> 
                    let result = sut.Analyze project

                    test <@ result.Namespaces |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withNamespacesTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                namespace Namespace.A 
                {
                    namespace Namespace.A.Nested { }
                }

                namespace Namespace.B { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Namespace Namespace.A 
                    Namespace Namespace.A.Nested           
                    End Namespace
                End Namespace

                Namespace Namespace.B
                End Namespace
            "])
        ]
        testList "Analyze/Namespace" [
            yield! testRepeat (withProjects contents)
                "should return namespaces when project has some"
                (fun sut project () -> 
                    let result = sut.Analyze project

                    test <@ result.Namespaces |> Seq.length = 3 @>)

            yield! testRepeatParameterized
                "should return namespace with correct name when project has some"[
                (withProjects contents, ("Namespace.A"))
                (withProjects contents, ("Namespace.A.Nested"))]
                (fun sut project name () -> 
                    let result = sut.Analyze project

                    test <@ result.Namespaces |> Seq.exists (fun c -> c.Name = name) @>)
        ]