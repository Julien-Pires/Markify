namespace Markify.CodeAnalyzer.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.CodeAnalyzer
open Markify.Tests.Extension

module RoslynAnalyzer_Projects_Tests =
    [<Tests>]
    let emptyProjectTests =
        testList "Analyze/Project" [
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
            (CSharp, ["
                    public class FooType { }
                    public class FooType { }
            "])
            (VisualBasic, ["
                    Public Class FooType
                    End Class
                    Public Class FooType
                    End Class
            "])
        ]
        testList "Analyze/Project" [
            yield! testRepeat (withProjects duplicates)
                "should returns no duplicate types"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = assemblies.Types |> Seq.groupBy (fun c -> "")//getFullname c)

                    test <@ (result |> Seq.length) = (assemblies.Types |> Seq.length) @>)
        ]