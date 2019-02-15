namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzer_Enum_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public enum FooType { }
                public class ParentType
                {
                    public enum NestedType { }
                    public class AnotherNestedType
                    {
                        public enum DeeperNestedType { }
                    }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Enum FooType
                End Enum
                Public Class ParentType
                    Public Enum NestedType
                    End Enum
                    Public Class AnotherNestedType
                        Public Enum DeeperNestedType
                        End Enum
                    End Class
                End Class
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should returns enum when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return enum with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized 
                "should return enum with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
        ]