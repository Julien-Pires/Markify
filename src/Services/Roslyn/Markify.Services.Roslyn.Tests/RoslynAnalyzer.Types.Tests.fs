namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Services.Roslyn
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
    let projectContentTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class FooType
                    {
                        public class NestedType
                        {
                            public class DeeperNestedType
                            {
                            }
                        }
                    }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Class FooType
                        Public Class NestedType
                            Public Class DeeperNestedType
                            End Class
                        End Class
                    End Class
                "]
            )
        ]

        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should returns types when project is not empty"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized "should return types with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized "should return types with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
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