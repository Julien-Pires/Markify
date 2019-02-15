namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzer_Struct_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public struct FooType
                {
                    public struct NestedType
                    {
                        public struct DeeperNestedType { }
                    }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure FooType
                    Public Structure NestedType
                        Public Structure DeeperNestedType
                        End Structure
                    End Structure
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects contents)
                "should returns struct when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return struct with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized 
                "should return struct with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
        ]