namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructPartial_Tests =
    [<Tests>]
    let mergingPartialTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial struct PartialType : IEnumerable
                {
                }
                public partial struct PartialType : IDisposable
                {
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Structure PartialType
                    Implements IEnumerable
                End Structure
                Partial Public Structure PartialType
                    Implements IDisposable
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should merge inherited types when identical partial structs exist"
                (fun sut project () -> 
                    let result = sut.Analyze project |> findStruct "PartialType"

                    test <@ result.Identity.BaseTypes |> Set = Set ["IEnumerable"; "IDisposable"] @>)

            yield! testRepeat (withProjects content)
                "should merge modifiers when identical partial structs exist"
                (fun sut project () -> 
                    let result = sut.Analyze project |> findStruct "PartialType"

                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax
                                                      |> Set
                                                      |> Set.isSubset (Set ["partial"]) @>)
        ]

    [<Tests>]
    let mergingMembersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public partial struct PartialType
                {
                    int FieldOne;
                    int PropertyOne { get; }
                    void MethodOne() { }
                    event EventHandler EventOne;
                }
                public partial struct PartialType
                {
                    int FieldTwo;
                    int PropertyTwo { get; }
                    void MethodTwo() { }
                    event EventHandler EventTwo;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Structure PartialType
                    Dim FieldOne As Integer
                    Property PropertyOne As Integer
                    Sub MethodOne()
                    End Sub
                    Event EventOne As EventHandler
                End Structure
                Partial Public Structure PartialType
                    Dim FieldTwo As Integer
                    Property PropertyTwo As Integer
                    Sub MethodTwo()
                    End Sub
                    Event EventTwo As EventHandler
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should merge fields when identical partial structs exist"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "PartialType"
                    
                    test <@ result.Fields |> Seq.map (fun c -> c.Name)
                                          |> Set
                                          |> Set.isSubset (Set ["FieldOne"; "FieldTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge properties when identical partial structs exist"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "PartialType"
                    
                    test <@ result.Properties |> Seq.map (fun c -> c.Name)
                                              |> Set
                                              |> Set.isSubset (Set ["PropertyOne"; "PropertyTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge methods when identical partial structs exist"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "PartialType"
                    
                    test <@ result.Methods |> Seq.map (fun c -> c.Identity.Name)
                                           |> Set
                                           |> Set.isSubset (Set ["MethodOne"; "MethodTwo"]) @>)

            yield! testRepeat (withProjects content)
                "should merge events when identical partial structs exist"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "PartialType"
                    
                    test <@ result.Events |> Seq.map (fun c -> c.Name)
                                          |> Set
                                          |> Set.isSubset (Set ["EventOne"; "EventTwo"]) @>)
        ]