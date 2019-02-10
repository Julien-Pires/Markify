namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesInheritanceTests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public struct ImplementInterfaceType : IDisposable { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Structure ImplementInterfaceType
                        Implements IDisposable
                    End Structure
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return base types when struct implements an interface"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "ImplementInterfaceType"
                        
                    test <@ Set result.Identity.BaseTypes |> Set.isSubset (Set ["IDisposable"]) @>)
        ]