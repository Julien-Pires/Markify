namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesEventsTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Swensen.Unquote
    open Xunit
    open TestHelper

    [<Theory>]
    [<ProjectData("Events", "TypeWithNoEvent")>]
    let ``Analyze should return definition with no events when type has none``(name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getEvents
        
        test <@ actual |> Seq.forall (Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents")>]
    let ``Analyze should return definition with events when type has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getEvents
        
        test <@ actual |> Seq.forall (Seq.isEmpty >> not) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Event")>]
    [<ProjectData("Events", "TypeWithEvents", "GenericEvent")>]
    let ``Analyze should return expected event name`` (name, event, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> Seq.map DefinitionsHelper.getEvents

        test <@ actual |> Seq.forall (fun c -> c |> Seq.exists (fun d -> d.Name = event)) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Event", "EventHandler")>]
    [<SingleLanguageProjectData("Events", ProjectLanguage.CSharp, "TypeWithEvents", "GenericEvent", "EventHandler<EventArgs>")>]
    [<SingleLanguageProjectData("Events", ProjectLanguage.VisualBasic, "TypeWithEvents", "GenericEvent", "EventHandler(Of EventArgs)")>]
    let ``Analyze should return expected type event`` (name, event, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getEvent event
        
        test <@ actual |> Seq.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Class;Struct", "Event", "private")>]
    [<ProjectData("Events", "TypeWithEvents", "Interface", "Event", "public")>]
    let ``Analyze should return default event access modifier when event has none`` (name, namespaces : string, event, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getEvent event

        test <@ actual |> Seq.forall (fun c -> c.AccessModifiers |> Seq.contains expected) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Class;Struct;Interface", "GenericEvent", "public")>]
    [<ProjectData("Events", "TypeWithEvents", "Class;Struct", "InternalEvent", "internal")>]
    [<ProjectData("Events", "TypeWithEvents", "Class;Struct", "PrivateEvent", "private")>]
    [<ProjectData("Events", "TypeWithEvents", "Class", "ProtectedInternalEvent", "protected;internal")>]
    [<ProjectData("Events", "TypeWithEvents", "Class", "ProtectedEvent  ", "protected")>]
    let ``Analyze should return event access modifiers when event has some`` (name, namespaces : string, event, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiersOld modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getEvent event

        test <@ actual |> Seq.map (fun c -> Set c.AccessModifiers)
                       |> Seq.forall (Set.isSubset expected) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Event")>]
    let ``Analyze should return no modifiers when event has none`` (name, event, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getEvent event

        test <@ actual |> Seq.forall (fun c -> c.Modifiers |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Events", "TypeWithEvents", "Class;Struct", "StaticEvent", "static")>]
    [<ProjectData("Events", "TypeWithEvents", "Class", "VirtualEvent", "virtual")>]
    [<ProjectData("Events", "TypeWithEvents", "Class", "SealedEvent", "sealed;override")>]
    [<ProjectData("Events", "AbstractTypeWithEvents", "Class", "AbstractEvent", "abstract")>]
    let ``Analyze should return modifiers when event has some`` (name, namespaces : string, event, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getMemberModifiers modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getEvent event

        test <@ actual |> Seq.map (fun c -> Set c.Modifiers)
                       |> Seq.forall (Set.isSubset expected) @>