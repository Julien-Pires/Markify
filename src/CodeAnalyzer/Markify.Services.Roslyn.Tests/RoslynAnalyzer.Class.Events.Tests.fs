namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassEvents_Tests =
    [<Tests>]
    let noEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutEvents {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutEvents
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no events when class has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "WithoutEvents"

                    test <@ result.Events |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withEventsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class SingleEvent 
                {
                    event EventHandler FirstEvent;
                }
                public class MultipleEvents
                {
                    event EventHandler FirstEvent;
                    event AnotherEventHandler SecondEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class SingleEvent
                    Event FirstEvent As EventHandler
                End Class
                Public Class MultipleEvents
                    Event FirstEvent As EventHandler
                    Event SecondEvent As AnotherEventHandler
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return events when class has some" [
                (withProjects content, ("SingleEvent", 1))
                (withProjects content, ("MultipleEvents", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Events |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct event name when class has some" [
                (withProjects content, ("SingleEvent", "FirstEvent"))
                (withProjects content, ("MultipleEvents", "SecondEvent"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Events |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct event type when class has some" [
                (withProjects content, ("SingleEvent", "FirstEvent", "EventHandler"))
                (withProjects content, ("MultipleEvents", "SecondEvent", "AnotherEventHandler"))]
                (fun sut project (name, event, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Events |> Seq.find (fun c -> c.Name = event)
                                          |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class AccessModifier 
                {
                    event EventHandler WithoutAccessModifier;
                    public event EventHandler PublicEvent;
                    internal event EventHandler InternalEvent;
                    protected event EventHandler ProtectedEvent;
                    protected internal event EventHandler ProtectedInternalEvent;
                    private event EventHandler PrivateEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class AccessModifier
                    Event WithoutAccessModifier As EventHandler
                    Public Event PublicEvent As EventHandler
                    Friend Event InternalEvent As EventHandler
                    Protected Event ProtectedEvent As EventHandler
                    Protected Friend Event ProtectedInternalEvent As EventHandler
                    Private Event PrivateEvent As EventHandler
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return correct class event access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicEvent", Set ["public"]))
                (withProjects content, ("InternalEvent", Set ["internal"]))
                (withProjects content, ("ProtectedEvent", Set ["protected"]))
                (withProjects content, ("ProtectedInternalEvent", Set ["protected"; "internal"]))
                (withProjects content, ("PrivateEvent", Set ["private"]))]
                (fun sut project (event, expected) () -> 
                    let object = sut.Analyze project |> findClass "AccessModifier"
                    let result = object.Events |> Seq.find (fun c -> c.Name = event)

                    test <@ result.AccessModifiers |> Set
                                                   |> Set.map normalizeSyntax
                                                   |> (=) expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public abstract class Modifiers 
                {
                    event EventHandler WithoutModifier;
                    static event EventHandler StaticEvent;
                    virtual event EventHandler VirtualEvent;
                    sealed event EventHandler SealedEvent;
                    override event EventHandler OverrideEvent;
                    abstract event EventHandler AbstractEvent;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public MustInherit Class Modifiers
                    Event WithoutModifier As EventHandler
                    Shared Event StaticEvent As EventHandler
                    Overridable Event VirtualEvent As EventHandler
                    NotOverridable Event SealedEvent As EventHandler
                    Overrides Event OverrideEvent As EventHandler
                    MustInherit Event AbstractEvent As EventHandler
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no modifier when class event has none"
                (fun sut project () -> 
                    let object = sut.Analyze project |> findClass "Modifiers"
                    let result = object.Events |> Seq.find (fun c -> c.Name = "WithoutModifier")

                    test <@ result.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return modifier when class event has one" [
                (withProjects content, ("StaticEvent", Set ["static"]))
                (withProjects content, ("VirtualEvent", Set ["virtual"]))
                (withProjects content, ("SealedEvent", Set ["sealed"]))
                (withProjects content, ("OverrideEvent", Set ["override"]))
                (withProjects content, ("AbstractEvent", Set ["abstract"]))]
                (fun sut project (event, expected) () -> 
                    let object = sut.Analyze project |> findClass "Modifiers"
                    let result = object.Events |> Seq.find (fun c -> c.Name = event)

                    test <@ result.Modifiers |> Set
                                             |> Set.map normalizeSyntax 
                                             |> (=) expected @>)
        ]