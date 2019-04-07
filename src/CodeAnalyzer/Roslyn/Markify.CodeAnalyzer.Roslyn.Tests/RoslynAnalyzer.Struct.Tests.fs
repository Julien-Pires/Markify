namespace Markify.CodeAnalyzer.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.CodeAnalyzer
open Markify.Tests.Extension

module RoslynAnalyzer_Struct_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                namespace RootNamespace 
                {
                    public struct RootType
                    {
                        public struct NestedType
                        {
                            public struct DeeperNestedType { }
                        }
                    }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Namespace RootNamespace
                    Public Structure RootType
                        Public Structure NestedType
                            Public Structure DeeperNestedType
                            End Structure
                        End Structure
                    End Structure
                End Namespace
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
                (withProjects contents, "RootType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                            
                    test <@ assemblies.Types |> Seq.exists (fun c -> c.Identity.Name = name) @>)
            
            yield! testRepeatParameterized 
                "should return struct with valid fullname" [
                (withProjects contents, "RootNamespace.RootType")
                (withProjects contents, "RootNamespace.RootType.NestedType")
                (withProjects contents, "RootNamespace.RootType.NestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project

                    test <@ assemblies.Types |> Seq.exists (doesNameMatch fullname) @>)
        ]