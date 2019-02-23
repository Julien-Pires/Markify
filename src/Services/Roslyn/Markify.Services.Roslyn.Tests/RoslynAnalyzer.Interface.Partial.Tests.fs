﻿namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfacePartial_Tests =
    [<Tests>]
    let mergingPartialTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial interface PartialType : IEnumerable
                {
                }
                public partial interface PartialType : IDisposable
                {
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Interface PartialType
                    Implements IEnumerable
                End Interface
                Partial Public Interface PartialType
                    Implements IDisposable
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should merge inherited types when identical partial interfaces exist"
                (fun sut project () -> 
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "PartialType"

                    test <@ result.Identity.BaseTypes |> Set = Set ["IEnumerable"; "IDisposable"] @>)

            yield! testRepeat (withProjects content)
                "should merge modifiers when identical partial interfaces exist"
                (fun sut project () -> 
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "PartialType"

                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax
                                                      |> Set
                                                      |> Set.isSubset (Set ["partial"]) @>)
        ]

    [<Tests>]
    let mergingMembersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial interface PartialType
                {
                    int PropertyOne { get; }
                    void MethodOne();
                    event EventHandler EventOne;
                }
                public partial interface PartialType
                {
                    int PropertyTwo { get; }
                    void MethodTwo();
                    event EventHandler EventTwo;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Interface PartialType
                    Property PropertyOne As Integer
                    Sub MethodOne()
                    Event EventOne As EventHandler
                End Interface
                Partial Public Interface PartialType
                    Property PropertyTwo As Integer
                    Sub MethodTwo()
                    Event EventTwo As EventHandler
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should merge properties when identical partial interfaces exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "PartialType"
                    
                    test <@ result.Properties |> Seq.map (fun c -> c.Name)
                                              |> Set
                                              |> Set.isSubset (Set ["PropertyOne"; "PropertyTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge methods when identical partial interfaces exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "PartialType"
                    
                    test <@ result.Methods |> Seq.map (fun c -> c.Identity.Name)
                                           |> Set
                                           |> Set.isSubset (Set ["MethodOne"; "MethodTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge events when identical partial interfaces exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "PartialType"
                    
                    test <@ result.Events |> Seq.map (fun c -> c.Name)
                                          |> Set
                                          |> Set.isSubset (Set ["EventOne"; "EventTwo"]) @>)
        ]