namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzer_Interface_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public interface FooType
                    {
                        public interface NestedType
                        {
                            public interface DeeperNestedType { }
                        }
                    }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Interface FooType
                        Public Interface NestedType
                            Public Interface DeeperNestedType
                            End Interface
                        End Interface
                    End Interface
                "]
            )
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects contents)
                "should returns interface when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return interface with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized 
                "should return interface with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
        ]