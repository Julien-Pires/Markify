namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzerTypesTests =
    [<Tests>]
    let emptyProjectTests =
        testList "Analyze" [
            yield! testFixture withSut [
                "should returns no type when project is empty",
                fun sut ->
                    let emptyProject = { 
                        Name = "EmptyProject"
                        Content = [] }
                    let result = sut.Analyze emptyProject

                    test <@ result.Types |> Seq.isEmpty @>
            ]
        ]

    [<Tests>]
    let duplicateContentTests =
        let duplicates = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class FooType { }
                    public class FooType { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Class FooType
                    End Class
                    Public Class FooType
                    End Class
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects duplicates)
                "should returns no duplicate types"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = assemblies.Types |> Seq.groupBy (fun c -> getFullname c.Identity)

                    test <@ (result |> Seq.length) = (assemblies.Types |> Seq.length) @>)
        ]