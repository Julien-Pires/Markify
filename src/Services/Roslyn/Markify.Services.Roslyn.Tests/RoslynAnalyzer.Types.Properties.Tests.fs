namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesPropertiesTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open TestHelper
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.CSharp, "FooType", 0)>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.CSharp, "FooType", 4)>]
    [<MultiProjectData("TypeMembers/StructProperties", ProjectLanguage.CSharp, "FooType", 10)>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.CSharp, "FooType", 13)>]
    [<MultiProjectData("TypesInformations/AllTypesSamples", ProjectLanguage.VisualBasic, "FooType", 0)>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", 4)>]
    [<MultiProjectData("TypeMembers/StructProperties", ProjectLanguage.VisualBasic, "FooType", 9)>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.VisualBasic, "FooType", 12)>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.CSharp, "FooType", "WithMultipleModifiersProperty", "protected;internal")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.VisualBasic, "FooType", "WithMultipleModifiersProperty", "Protected;Friend")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "StaticProperty", "static")>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.CSharp, "FooType", "SealedProperty", "sealed;override")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "StaticProperty", "Shared")>]
    [<MultiProjectData("TypeMembers/ClassProperties", ProjectLanguage.VisualBasic, "FooType", "SealedProperty", "NotOverridable;Overrides")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "int")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Object")>]
    let ``Analyze should return expected property type`` (name, property, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) = 
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let propertyDefinition = getProperty library.Types name property
                propertyDefinition.Type::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithInitialValueProperty", "1")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithInitialValueProperty", "1")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", null)>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", "public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", null)>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", "Public")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", "public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithModifierProperty", "protected")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithSetterModifierProperty", "internal")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", "Public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithModifierProperty", "Protected")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithSetterModifierProperty", "Friend")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "WriteOnlyProperty", null)>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithExpressionBody", "public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "WriteOnlyProperty", null)>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
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
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.CSharp, "FooType", "ReadOnlyProperty", "public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.CSharp, "FooType", "AutoProperty", "public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithModifierProperty", "protected")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithNoModifierProperty", "private")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.CSharp, "FooType", "WithGetterModifierProperty", "internal")>]
    [<MultiProjectData("TypeMembers/AllTypesProperties", ProjectLanguage.VisualBasic, "FooType", "ReadOnlyProperty", "Public")>]
    [<MultiProjectData("TypeMembers/InterfaceProperties", ProjectLanguage.VisualBasic, "FooType", "AutoProperty", "Public")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithModifierProperty", "Protected")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithNoModifierProperty", "Private")>]
    [<MultiProjectData("TypeMembers/ContainerProperties", ProjectLanguage.VisualBasic, "FooType", "WithGetterModifierProperty", "Friend")>]
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