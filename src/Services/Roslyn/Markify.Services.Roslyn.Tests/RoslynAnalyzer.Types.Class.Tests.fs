namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzer_Class_Tests =
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
        testList "Analyze/Class" [
            yield! testRepeat (withProjects contents)
                "should returns class when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return class with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized 
                "should return class with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
        ]