namespace Markify.Roslyn.Tests

module RoslynAnalyzerTypesPropertiesTests =
    open System
    open Markify.Models.IDE
    open Markify.Core.Analyzers
    open Markify.Models.Definitions
    open Markify.Roslyn
    open TestHelper
    open DefinitionsHelper
    open Attributes
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooType", 0)>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.CSharp, "FooType", 4)>]
    [<MultiProjectData("StructProperties", ProjectLanguage.CSharp, "FooType", 10)>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.CSharp, "FooType", 13)>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooType", 0)>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", 4)>]
    [<MultiProjectData("StructProperties", ProjectLanguage.VisualBasic, "FooType", 9)>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.VisualBasic, "FooType", 12)>]
    let ``Analyze should return expected property count`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getProperties
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty")>]
    let ``Analyze should return expected property name`` (name, property, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getProperties
                    |> Seq.filter (fun d -> d.Name = property)
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) 1) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.CSharp, "FooType", "WithMultipleModifiersProperty", "protected;internal")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.VisualBasic, "FooType", "WithMultipleModifiersProperty", "Protected;Friend")>]
    let ``Analyze should return all access modifiers when property has some`` (name, property, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                let accessModifiers = Set propertyDefinition.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "StaticProperty", "static")>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.CSharp, "FooType", "SealedProperty", "sealed;override")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "StaticProperty", "Shared")>]
    [<MultiProjectData("ClassProperties", ProjectLanguage.VisualBasic, "FooType", "SealedProperty", "NotOverridable;Overrides")>]
    let ``Analyze should return all modifiers when property has some`` (name, property, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                let accessModifiers = Set propertyDefinition.Modifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "int")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Object")>]
    let ``Analyze should return expected property type`` (name, property, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) = 
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                propertyDefinition.Type::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithInitialValueProperty", "1")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithInitialValueProperty", "1")>]
    let ``Analyze should return default value when type has some`` (name, property, value, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = 
            match String.IsNullOrWhiteSpace value with
            | true -> None
            | false -> Some value
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                propertyDefinition.DefaultValue::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", null)>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", "public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", null)>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", "Public")>]
    let ``Analyze should return write accessor when property has one`` (name, property, accessor, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected =
            match accessor with
            | null -> None
            | x -> Some { AccessorDefinition.AccessModifiers = [x] }
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                propertyDefinition.IsWrite::acc) []

        test <@ actual |> List.forall ((isSemanticEqual) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", "public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithModifierProperty", "protected")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithSetterModifierProperty", "internal")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", "Public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithModifierProperty", "Protected")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithSetterModifierProperty", "Friend")>]
    let ``Analyze should return write accessor modifiers when accessor has some`` (name, property, (modifiers : string), sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                let accessModifiers = Set propertyDefinition.IsWrite.Value.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", null)>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithExpressionBody", "public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", null)>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
    let ``Analyze should return read accessor when property has one`` (name, property, accessor, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected =
            match accessor with
            | null -> None
            | x -> Some { AccessorDefinition.AccessModifiers = [x] }
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                propertyDefinition.IsRead::acc) []

        test <@ actual |> List.forall ((isSemanticEqual) expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithModifierProperty", "protected")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithGetterModifierProperty", "internal")>]
    [<MultiProjectData("AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
    [<MultiProjectData("InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithModifierProperty", "Protected")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithGetterModifierProperty", "Friend")>]
    let ``Analyze should return read accessor modifiers when accessor has some`` (name, property, (modifiers : string), sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                let accessModifiers = Set propertyDefinition.IsRead.Value.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>