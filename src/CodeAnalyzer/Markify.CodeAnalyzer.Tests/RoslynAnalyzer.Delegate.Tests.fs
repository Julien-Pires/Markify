namespace Markify.CodeAnalyzer.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.CodeAnalyzer
open Markify.Tests.Extension

module RoslynAnalyzer_Delegate_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (CSharp, ["
                namespace RootNamespace 
                {
                    public delegate void RootType();
                    public class ParentType
                    {
                        public delegate void NestedType();
                        public class AnotherNestedType
                        {
                            public delegate void DeeperNestedType();
                        }
                    }
                }
            "])
            (VisualBasic, ["
                Namespace RootNamespace
                    Public Delegate Sub RootType()
                    Public Class ParentType
                        Public Delegate Sub NestedType()
                        Public Class AnotherNestedType
                            Public Delegate Sub DeeperNestedType()
                        End Class
                    End Class
                End Namespace
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects contents)
                "should returns delegate when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized 
                "should return delegate with valid name" [
                (withProjects contents, "RootType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                            
                    test <@ assemblies.Types |> Seq.exists (fun c -> c.Name = name) @>)
            
            yield! testRepeatParameterized 
                "should return delegate with valid fullname" [
                (withProjects contents, "RootNamespace.RootType")
                (withProjects contents, "RootNamespace.ParentType.NestedType")
                (withProjects contents, "RootNamespace.ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project

                    test <@ assemblies.Types |> Seq.exists (doesNameMatch fullname) @>)
        ]