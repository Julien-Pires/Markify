namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler
open Markify.Tests.Extension

module RoslynAnalyzer_Interface_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                namespace RootNamespace 
                {
                    public interface RootType
                    {
                        public interface NestedType
                        {
                            public interface DeeperNestedType { }
                        }
                    }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Namespace RootNamespace
                    Public Interface RootType
                        Public Interface NestedType
                            Public Interface DeeperNestedType
                            End Interface
                        End Interface
                    End Interface
                End Namespace
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects contents)
                "should returns interface when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return interface with valid name" [
                (withProjects contents, "RootType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                            
                    test <@ assemblies.Types |> Seq.exists (fun c -> c.Identity.Name = name) @>)
            
            yield! testRepeatParameterized 
                "should return interface with valid fullname" [
                (withProjects contents, "RootNamespace.RootType")
                (withProjects contents, "RootNamespace.RootType.NestedType")
                (withProjects contents, "RootNamespace.RootType.NestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project

                    test <@ assemblies.Types |> Seq.exists (doesNameMatch fullname) @>)
        ]