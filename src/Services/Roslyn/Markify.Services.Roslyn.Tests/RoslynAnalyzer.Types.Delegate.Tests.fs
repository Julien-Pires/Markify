namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Domain.Compiler

module RoslynAnalyzerDelegateTests =
    [<Tests>]
    let projectContentTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void FooType();

                    public class ParentType
                    {
                        public delegate void NestedType();

                        public class AnotherNestedType
                        {
                            public delegate void DeeperNestedType();
                        }
                    }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub FooType()

                    Public Class ParentType
                        Public Delegate Sub NestedType()

                        Public Class AnotherNestedType
                            Public Delegate Sub DeeperNestedType()
                        End Class
                    End Class
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should returns delegate when project has some"
                (fun sut project () ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>)

            yield! testRepeatParameterized "should return delegate with valid name" [
                (withProjects contents, "FooType")
                (withProjects contents, "NestedType")
                (withProjects contents, "DeeperNestedType")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies name
                            
                    test <@ result |> Seq.length > 0 @>)
            
            yield! testRepeatParameterized "should return delegate with valid fullname" [
                (withProjects contents, "FooType")
                (withProjects contents, "ParentType.NestedType")
                (withProjects contents, "ParentType.AnotherNestedType.DeeperNestedType")]
                (fun sut project fullname () ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies fullname

                    test <@ result |> Seq.length > 0 @>)
        ]