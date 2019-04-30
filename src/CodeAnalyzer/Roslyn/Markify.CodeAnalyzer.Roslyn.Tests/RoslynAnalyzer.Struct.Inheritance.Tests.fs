namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public struct ImplementInterfaceType : IDisposable { }
                public struct ImplementMultipleInterfaceType : IDisposable, IEnumerable { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure ImplementInterfaceType
                    Implements IDisposable
                End Structure
                Public Structure ImplementMultipleInterfaceType
                    Implements IDisposable, IEnumerable
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return base types when struct implements interfaces" [
                (withProjects contents, ("ImplementInterfaceType", Set ["IDisposable"]))
                (withProjects contents, ("ImplementMultipleInterfaceType", Set ["IDisposable"; "IEnumerable"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ Set result.BaseType = expected @>)
        ]