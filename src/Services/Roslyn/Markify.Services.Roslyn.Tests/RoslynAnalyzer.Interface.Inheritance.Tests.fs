﻿namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public interface ImplementInterfaceType : IDisposable { }

                    public interface ImplementMultipleInterfaceType : IDisposable, IEnumerable { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Interface ImplementInterfaceType
                        Implements IDisposable
                    End Interface

                    Public Interface ImplementMultipleInterfaceType
                        Implements IDisposable, IEnumerable
                    End Interface
                "]
            )
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return base types when struct implements interfaces" [
                (withProjects contents, ("ImplementInterfaceType", Set ["IDisposable"]))
                (withProjects contents, ("ImplementMultipleInterfaceType", Set ["IDisposable"; "IEnumerable"]))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                        
                    test <@ Set result.Identity.BaseTypes |> Set.isSubset expected @>)
        ]