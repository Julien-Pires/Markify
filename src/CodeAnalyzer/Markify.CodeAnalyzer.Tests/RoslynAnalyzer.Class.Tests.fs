namespace Markify.CodeAnalyzer.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.CodeAnalyzer
open Markify.Tests.Extension

module RoslynAnalyzer_Class_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (CSharp, ["
                namespace RootNamespace 
                {
                    public class RootType
                    {
                        public class NestedType
                        {
                            public class DeeperNestedType
                            {
                            }
                        }
                    }
                }
            "])
            (VisualBasic, ["
                Namespace RootNamespace
                    Public Class RootType
                        Public Class NestedType
                            Public Class DeeperNestedType
                            End Class
                        End Class
                    End Class
                End Namespace
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects contents)
                "should returns class when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return class with valid name" [
                (withProjects contents, "RootType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                            
                    test <@ assemblies.Types |> Seq.exists (fun c -> c.Name = name) @>)
            
            yield! testRepeatParameterized
                "should return class with valid fullname" [
                (withProjects contents, "RootNamespace.RootType")
                (withProjects contents, "RootNamespace.RootType.NestedType")
                (withProjects contents, "RootNamespace.RootType.NestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project

                    test <@ assemblies.Types |> Seq.exists (doesNameMatch fullname) @>)
        ]