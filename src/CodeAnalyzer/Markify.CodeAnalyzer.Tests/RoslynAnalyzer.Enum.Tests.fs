namespace Markify.CodeAnalyzer.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.CodeAnalyzer
open Markify.Tests.Extension

module RoslynAnalyzer_Enum_Tests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (CSharp, ["
                namespace RootNamespace 
                {
                    public enum RootType { }
                    public class ParentType
                    {
                        public enum NestedType { }
                        public class AnotherNestedType
                        {
                            public enum DeeperNestedType { }
                        }
                    }
                }
            "])
            (VisualBasic, ["
                Namespace RootNamespace
                    Public Enum RootType
                    End Enum
                    Public Class ParentType
                        Public Enum NestedType
                        End Enum
                        Public Class AnotherNestedType
                            Public Enum DeeperNestedType
                            End Enum
                        End Class
                    End Class
                End Namespace
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
                (withProjects contents, "RootType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                            
                    test <@ assemblies.Types |> Seq.exists (fun c -> c.Name = name) @>)
            
            yield! testRepeatParameterized 
                "should return enum with valid fullname" [
                (withProjects contents, "RootNamespace.RootType")
                (withProjects contents, "RootNamespace.ParentType.NestedType")
                (withProjects contents, "RootNamespace.ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project

                    test <@ assemblies.Types |> Seq.exists (doesNameMatch fullname) @>)
        ]