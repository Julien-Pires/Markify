namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructEvents_Tests =
    [<Tests>]
    let noEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutEvents {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutEvents
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no events when struct has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "WithoutEvents"

                    test <@ result.Events |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct SingleEvent 
                {
                    event EventHandler FirstEvent;
                }
                public struct MultipleEvents
                {
                    event EventHandler FirstEvent;
                    event AnotherEventHandler SecondEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleEvent
                    Event FirstEvent As EventHandler
                End Structure
                Public Structure MultipleEvents
                    Event FirstEvent As EventHandler
                    Event SecondEvent As AnotherEventHandler
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return events when struct has some" [
                (withProjects content, ("SingleEvent", 1))
                (withProjects content, ("MultipleEvents", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Events |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct event name when struct has some" [
                (withProjects content, ("SingleEvent", "FirstEvent"))
                (withProjects content, ("MultipleEvents", "SecondEvent"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Events |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct event type when struct has some" [
                (withProjects content, ("SingleEvent", "FirstEvent", "EventHandler"))
                (withProjects content, ("MultipleEvents", "SecondEvent", "AnotherEventHandler"))]
                (fun sut project (name, event, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Events |> Seq.find (fun c -> c.Name = event)
                                          |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct AccessModifier 
                {
                    event EventHandler WithoutAccessModifier;
                    public event EventHandler PublicEvent;
                    internal event EventHandler InternalEvent;
                    private event EventHandler PrivateEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure AccessModifier
                    Event WithoutAccessModifier As EventHandler
                    Public Event PublicEvent As EventHandler
                    Friend Event InternalEvent As EventHandler
                    Private Event PrivateEvent As EventHandler
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return correct struct event access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicEvent", Set ["public"]))
                (withProjects content, ("InternalEvent", Set ["internal"]))
                (withProjects content, ("PrivateEvent", Set ["private"]))]
                (fun sut project (event, expected) () -> 
                    let object = sut.Analyze project |> findStruct "AccessModifier"
                    let result = object.Events |> Seq.find (fun c -> c.Name = event)

                    test <@ result.AccessModifiers |> Seq.map normalizeSyntax 
                                                   |> Set
                                                   |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct Modifiers 
                {
                    event EventHandler WithoutModifier;
                    static event EventHandler StaticEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure Modifiers
                    Event WithoutModifier As EventHandler
                    Shared Event StaticEvent As EventHandler
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no modifier when struct event has none"
                (fun sut project () -> 
                    let object = sut.Analyze project |> findStruct "Modifiers"
                    let result = object.Events |> Seq.find (fun c -> c.Name = "WithoutModifier")

                    test <@ result.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return modifier when struct event has one" [
                (withProjects content, ("StaticEvent", Set ["static"]))]
                (fun sut project (event, expected) () -> 
                    let object = sut.Analyze project |> findStruct "Modifiers"
                    let result = object.Events |> Seq.find (fun c -> c.Name = event)

                    test <@ result.Modifiers |> Seq.map normalizeSyntax 
                                             |> Set
                                             |> Set.isSubset expected @>)
        ]