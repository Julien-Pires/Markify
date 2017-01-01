namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesEventsTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Swensen.Unquote
    open Xunit
    
    let getEvents = function
        | Class c | Struct c | Interface c -> c.Events
        | _ -> Seq.empty

    let findEvent (types : TypeDefinition seq) typeName eventName =
        types
        |> Seq.find (fun d -> d.Identity.Name = typeName)
        |> getEvents
        |> Seq.find (fun d -> d.Name = eventName)

    [<Theory>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", 0)>]
    [<MultiProjectData("ClassEvents", ProjectLanguage.CSharp, "FooType", 8)>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", 0)>]
    [<MultiProjectData("ClassEvents", ProjectLanguage.VisualBasic, "FooType", 6)>]
    let ``Analyze should return expected type events count`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun c -> c.Identity.Name = name)
                    |> getEvents
                count::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.CSharp, "FooType", "PrivateEvent")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.CSharp, "FooType", "ExplicitEvent")>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.VisualBasic, "FooType", "PrivateEvent")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.VisualBasic, "FooType", "ExplicitEvent")>]
    let ``Analyze should return expected event name`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let events =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getEvents
                    |> Seq.filter (fun d -> d.Name = expected)
                events::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = 1) @>

    [<Theory>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.CSharp, "FooType", "PrivateEvent", "EventHandler")>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.CSharp, "FooType", "GenericEvent", "EventHandler<EventArgs>")>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.VisualBasic, "FooType", "PrivateEvent", "EventHandler")>]
    [<MultiProjectData("AllTypesEvents", ProjectLanguage.VisualBasic, "FooType", "GenericEvent", "EventHandler(Of EventArgs)")>]
    let ``Analyze should return expected type event`` (name, eventName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let event = findEvent library.Types name eventName
                event::acc) []

        test <@ actual |> List.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.CSharp, "FooType", "PrivateEvent", "private")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.CSharp, "FooType", "InternalEvent", "internal")>]
    [<MultiProjectData("ClassEvents", ProjectLanguage.CSharp, "FooType", "ProtectedInternalEvent", "protected;internal")>]
    [<MultiProjectData("InterfaceEvents", ProjectLanguage.CSharp, "FooType", "GenericEvent", "public")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.VisualBasic, "FooType", "PrivateEvent", "Private")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.VisualBasic, "FooType", "InternalEvent", "Friend")>]
    [<MultiProjectData("ClassEvents", ProjectLanguage.VisualBasic, "FooType", "ProtectedInternalEvent", "Protected;Friend")>]
    [<MultiProjectData("InterfaceEvents", ProjectLanguage.VisualBasic, "FooType", "GenericEvent", "Public")>]
    let ``Analyze should return expected event access modifiers`` (name, eventName, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let event = findEvent library.Types name eventName
                let accessModifiers = Set event.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.CSharp, "FooType", "PrivateEvent", "")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.CSharp, "FooType", "StaticEvent", "static")>]
    [<MultiProjectData("ClassEvents", ProjectLanguage.CSharp, "FooType", "AbstractEvent", "sealed;override")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.VisualBasic, "FooType", "PrivateEvent", "")>]
    [<MultiProjectData("ContainerEvents", ProjectLanguage.VisualBasic, "FooType", "StaticEvent", "Shared")>]
    let ``Analyze should return expected event modifiers`` (name, eventName, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let event = findEvent library.Types name eventName
                let accessModifiers = Set event.Modifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>