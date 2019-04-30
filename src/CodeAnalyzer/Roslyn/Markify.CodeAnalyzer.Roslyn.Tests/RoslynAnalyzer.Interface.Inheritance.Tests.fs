namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public interface ImplementInterfaceType : IDisposable { }
                public interface ImplementMultipleInterfaceType : IDisposable, IEnumerable { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface ImplementInterfaceType
                    Inherits IDisposable
                End Interface
                Public Interface ImplementMultipleInterfaceType
                    Inherits IDisposable, IEnumerable
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return base types when interface inherits interfaces" [
                (withProjects contents, ("ImplementInterfaceType", Set ["IDisposable"]))
                (withProjects contents, ("ImplementMultipleInterfaceType", Set ["IDisposable"; "IEnumerable"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ Set result.BaseType = expected @>)
        ]