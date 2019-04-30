namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceEvents_Tests =
    [<Tests>]
    let noEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutEvents {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutEvents
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no events when interface has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findInterface "WithoutEvents"

                    test <@ result.Events |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface SingleEvent 
                {
                    event EventHandler FirstEvent;
                }
                public interface MultipleEvents
                {
                    event EventHandler FirstEvent;
                    event AnotherEventHandler SecondEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface SingleEvent
                    Event FirstEvent As EventHandler
                End Interface
                Public Interface MultipleEvents
                    Event FirstEvent As EventHandler
                    Event SecondEvent As AnotherEventHandler
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return events when interface has some" [
                (withProjects content, ("SingleEvent", 1))
                (withProjects content, ("MultipleEvents", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findInterface name

                    test <@ result.Events |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct event name when interface has some" [
                (withProjects content, ("SingleEvent", "FirstEvent"))
                (withProjects content, ("MultipleEvents", "SecondEvent"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findInterface name

                    test <@ result.Events |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct event type when interface has some" [
                (withProjects content, ("SingleEvent", "FirstEvent", "EventHandler"))
                (withProjects content, ("MultipleEvents", "SecondEvent", "AnotherEventHandler"))]
                (fun sut project (name, event, expected) () ->
                    let result = sut.Analyze project |> findInterface name

                    test <@ result.Events |> Seq.find (fun c -> c.Name = event)
                                          |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface AccessModifier 
                {
                    event EventHandler WithoutAccessModifier;
                    public event EventHandler PublicEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface AccessModifier
                    Event WithoutAccessModifier As EventHandler
                    Public Event PublicEvent As EventHandler
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return correct interface event access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["public"]))
                (withProjects content, ("PublicEvent", Set ["public"]))]
                (fun sut project (event, expected) () -> 
                    let info = sut.Analyze project |> findInterface "AccessModifier"
                    let result = info.Events |> Seq.find (fun c -> c.Name = event)

                    test <@ result.AccessModifiers |> Set
                                                   |> Set.map normalizeSyntax 
                                                   |> (=) expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface Modifiers 
                {
                    event EventHandler WithoutModifier;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface Modifiers
                    Event WithoutModifier As EventHandler
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no modifier when interface event has none"
                (fun sut project () -> 
                    let info = sut.Analyze project |> findInterface "Modifiers"
                    let result = info.Events |> Seq.find (fun c -> c.Name = "WithoutModifier")

                    test <@ result.Modifiers |> Seq.isEmpty @>)
        ]