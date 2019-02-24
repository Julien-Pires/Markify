namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassPartial_Tests =
    [<Tests>]
    let mergingPartialTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial sealed class PartialType : List
                {
                }
                public partial class PartialType : IDisposable
                {
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public NotInheritable Class PartialType
                    Inherits List
                End Class
                Partial Public Class PartialType
                    Implements IDisposable
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should merge inherited types when identical partial classes exist"
                (fun sut project () -> 
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"

                    test <@ result.Identity.BaseTypes |> Set = Set ["List"; "IDisposable"] @>)

            yield! testRepeat (withProjects content)
                "should merge modifiers when identical partial classes exist"
                (fun sut project () -> 
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"

                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax
                                                      |> Set
                                                      |> Set.isSubset (Set ["sealed"; "partial"]) @>)
        ]

    [<Tests>]
    let mergingMembersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial class PartialType
                {
                    int FieldOne;
                    int PropertyOne { get; }
                    void MethodOne() { }
                    event EventHandler EventOne;
                }
                public partial class PartialType
                {
                    int FieldTwo;
                    int PropertyTwo { get; }
                    void MethodTwo() { }
                    event EventHandler EventTwo;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Class PartialType
                    Dim FieldOne As Integer
                    Property PropertyOne As Integer
                    Sub MethodOne()
                    End Sub
                    Event EventOne As EventHandler
                End Class
                Partial Public Class PartialType
                    Dim FieldTwo As Integer
                    Property PropertyTwo As Integer
                    Sub MethodTwo()
                    End Sub
                    Event EventTwo As EventHandler
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should merge fields when identical partial classes exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"
                    
                    test <@ result.Fields |> Seq.map (fun c -> c.Name)
                                          |> Set
                                          |> Set.isSubset (Set ["FieldOne"; "FieldTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge properties when identical partial classes exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"
                    
                    test <@ result.Properties |> Seq.map (fun c -> c.Name)
                                              |> Set
                                              |> Set.isSubset (Set ["PropertyOne"; "PropertyTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge methods when identical partial classes exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"
                    
                    test <@ result.Methods |> Seq.map (fun c -> c.Identity.Name)
                                           |> Set
                                           |> Set.isSubset (Set ["MethodOne"; "MethodTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge events when identical partial classes exist"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "PartialType"
                    
                    test <@ result.Events |> Seq.map (fun c -> c.Name)
                                          |> Set
                                          |> Set.isSubset (Set ["EventOne"; "EventTwo"]) @>)
        ]