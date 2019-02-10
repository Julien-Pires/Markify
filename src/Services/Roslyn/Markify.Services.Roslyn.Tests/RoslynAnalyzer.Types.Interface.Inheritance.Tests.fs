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
                    public interface ImplementInterfaceType : IDisposable { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Interface ImplementInterfaceType
                        Implements IDisposable
                    End Interface
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return base types when interface inherits an interface"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "ImplementInterfaceType"
                        
                    test <@ Set result.Identity.BaseTypes |> Set.isSubset (Set ["IDisposable"]) @>)
        ]