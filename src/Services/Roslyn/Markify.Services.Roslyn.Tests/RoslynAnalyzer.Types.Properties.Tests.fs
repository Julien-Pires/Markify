namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesPropertiesTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Properties", "TypeWithNoProperties")>]
    let ``Analyze should return definition with no properties when type has none``(name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getProperties
        
        test <@ actual |> Seq.forall (Seq.isEmpty) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties")>]
    let ``Analyze should return definition with properties when type has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getProperties
        
        test <@ actual |> Seq.forall (Seq.isEmpty >> not) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty")>]
    [<ProjectData("Properties", "TypeWithProperties", "ReadOnlyProperty")>]
    [<ProjectData("Properties", "TypeWithProperties", "WriteOnlyProperty")>]
    let ``Analyze should return expected property name`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getProperties

        test <@ actual |> Seq.forall (fun c -> c |> Seq.exists (fun d -> d.Name = property)) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "WithNoModifierProperty", "private")>]
    [<ProjectData("Properties", "TypeWithProperties", "Interface", "WithNoModifierProperty", "public")>]
    let ``Analyze should return default property access modifier when property has none`` (name, namespaces : string, property, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.AccessModifiers |> Seq.contains expected) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct;Interface", "AutoProperty", "public")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "InternalProperty", "internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "PrivateProperty", "private")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "ProtectedInternalProperty", "protected;internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "ProtectedProperty", "protected")>]
    let ``Analyze should return property access modifiers when property has some`` (name, namespaces : string, property, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiers modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.map (fun c -> Set c.AccessModifiers)
                       |> Seq.forall (Set.isSubset expected) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty")>]
    let ``Analyze should return no modifiers when property has none`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.Modifiers |> Seq.isEmpty) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "StaticProperty", "static")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "VirtualProperty", "virtual")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "SealedProperty", "sealed;override")>]
    let ``Analyze should return modifiers when property has some`` (name, namespaces : string, property, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getMemberModifiers modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.map (fun c -> Set c.Modifiers)
                       |> Seq.forall (Set.isSubset expected) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty", "Int32")>]
    let ``Analyze should return property type`` (name, property, expected, sut : RoslynAnalyzer, project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property
        
        test <@ actual |> Seq.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty")>]
    let ``Analyze should return no default value when property has none`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.DefaultValue.IsNone) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "WithInitialValueProperty", "1")>]
    let ``Analyze should return default value when property has some`` (name, namespaces : string, property, value, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.DefaultValue = Some value) @>
    
    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "ReadOnlyProperty")>]
    let ``Analyze should return no write accessor when property is read-only`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.IsWrite.IsNone) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty")>]
    [<ProjectData("Properties", "TypeWithProperties", "WriteOnlyProperty")>]
    let ``Analyze should return write accessor when property has one`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.IsWrite.IsSome) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "WithSetterModifierProperty", "internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct;Interface", "WriteOnlyProperty", "public")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct;Interface", "AutoProperty", "public")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "InternalProperty", "internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "PrivateProperty", "private")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "ProtectedProperty", "protected")>]
    let ``Analyze should return write accessor modifiers when accessor has some`` (name, namespaces : string, property, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.map (fun c -> c.IsWrite.Value.AccessModifiers)
                       |> Seq.forall (fun c -> c |> Seq.contains expected) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "WriteOnlyProperty")>]
    let ``Analyze should return no read accessor when property is write-only`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.IsRead.IsNone) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "AutoProperty")>]
    [<ProjectData("Properties", "TypeWithProperties", "ReadOnlyProperty")>]
    [<SingleLanguageProjectData("Properties", ProjectLanguage.CSharp, "TypeWithProperties", "WithExpressionBody")>]
    let ``Analyze should return read accessor when property has one`` (name, property, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.forall (fun c -> c.IsRead.IsSome) @>

    [<Theory>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "WithGetterModifierProperty", "internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct;Interface", "ReadOnlyProperty", "public")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct;Interface", "AutoProperty", "public")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "InternalProperty", "internal")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class;Struct", "PrivateProperty", "private")>]
    [<ProjectData("Properties", "TypeWithProperties", "Class", "ProtectedProperty", "protected")>]
    let ``Analyze should return read accessor modifiers when accessor has some`` (name, namespaces : string, property, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.map (fun c -> c.IsRead.Value.AccessModifiers)
                       |> Seq.forall (fun c -> c |> Seq.contains expected) @>