namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesMethodsTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Methods", "TypeWithNoMethods")>]
    let ``Analyze should return definition with no methods when type has none``(name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getMethods
        
        test <@ actual |> Seq.forall (Seq.isEmpty) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods")>]
    let ``Analyze should return definition with methods when type has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getMethods
        
        test <@ actual |> Seq.forall (Seq.isEmpty >> not) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithoutParameters")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithGenericParameters")>]
    let ``Analyze should return expected method name`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getMethods

        test <@ actual |> Seq.forall (fun c -> c |> Seq.exists (fun d -> d.Identity.Name = method)) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct", "WithNoModifierMethod", "private")>]
    [<ProjectData("Methods", "TypeWithMethods", "Interface", "WithNoModifierMethod", "public")>]
    let ``Analyze should return default method access modifier when method has none`` (name, namespaces : string, method, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getMethod method

        test <@ actual |> Seq.forall (fun c -> c.Identity.AccessModifiers |> Seq.contains expected) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct;Interface", "WithoutParameters", "public")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct", "InternalMethod", "internal")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct", "PrivateMethod", "private")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class", "ProtectedInternalMethod", "protected;internal")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class", "ProtectedMethod", "protected")>]
    let ``Analyze should return method access modifiers when method has some`` (name, namespaces : string, method, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiersOld modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getMethod method

        test <@ actual |> Seq.map (fun c -> Set c.Identity.AccessModifiers)
                       |> Seq.forall (Set.isSubset expected) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithoutParameters")>]
    let ``Analyze should return no modifiers when property has none`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method

        test <@ actual |> Seq.forall (fun c -> c.Identity.Modifiers |> Seq.isEmpty) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct", "StaticMethod", "static")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class;Struct", "PartialMethod", "partial")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class", "VirtualMethod", "virtual")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class", "SealedMethod", "sealed")>]
    [<ProjectData("Methods", "TypeWithMethods", "Class", "SealedMethod", "override")>]
    let ``Analyze should return modifiers when property has some`` (name, namespaces : string, method, modifiers, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getMemberModifiers project.Language modifiers
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getMethod method

        test <@ actual |> Seq.forall (fun c -> c.Identity.Modifiers |> Seq.contains expected) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithoutParameters")>]
    let ``Analyze should return no generic parameters when method has none`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method

        <@ actual |> Seq.forall (fun c -> c.Identity.Parameters |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithOneParameter")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithMultipleParameters")>]
    let ``Analyze should return generic parameters when method has some`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method

        <@ actual |> Seq.forall (fun c -> c.Identity.Parameters |> (Seq.isEmpty >> not)) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "SingleGenericType")>]
    let ``Analyze should return no generic constraint when generic method has none`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.map (fun c -> c.Identity.Parameters)

        <@ actual |> Seq.collect id
                  |> Seq.forall (fun c -> c.Constraints |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "MultipleGenericType")>]
    let ``Analyze should return generic constraint when generic method has some`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.map (fun c -> c.Identity.Parameters)

        <@ actual |> Seq.collect id
                  |> Seq.forall (fun c -> c.Constraints |> (Seq.isEmpty >> not)) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "MultipleGenericType", "T", "Int32")>]
    [<ProjectData("Methods", "TypeWithMethods", "MultipleGenericType", "Y", "Int32")>]
    let ``Analyze should return expected generic constraint`` (name, method, generic, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Identity.Parameters)
            |> Seq.filter (fun c -> c.Name = generic)

        <@ actual |> Seq.forall (fun c -> c.Constraints |> Seq.contains expected) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithoutParameters")>]
    let ``Analyze should return no parameters when method has none`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method

        <@ actual |> Seq.forall (fun c -> c.Parameters |> Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithOneParameter")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithMultipleParameters")>]
    let ``Analyze should return parameters when method has some`` (name, method, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method

        <@ actual |> Seq.forall (fun c -> c.Parameters |> (Seq.isEmpty >> not)) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithOneParameter", "foo")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithMultipleParameters", "bar")>]
    let ``Analyze should return expected method parameter name`` (name, method, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method

        test <@ actual |> Seq.forall (fun c -> c.Parameters |> Seq.exists (fun c -> c.Name = parameter)) @>
    
    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithMultipleParameters", "foo", "Int32")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithMultipleParameters", "bar", "Single")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithGenericParameters`1", "foo", "T")>]
    let ``Analyze should return expected method parameter type`` (name, method, parameter, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Parameters)
            |> Seq.filter (fun c -> c.Name = parameter)

        <@ actual |> Seq.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithOneParameter", "foo")>]
    let ``Analyze should return no modifier when delegate parameter has none`` (name, method, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Parameters)
            |> Seq.filter (fun c -> c.Name = parameter)

        <@ actual |> Seq.forall (fun c -> c.Modifier = None) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithParametersModifiers", "foo", "ref")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithParametersModifiers", "foo", "out")>]
    let ``Analyze should return modifiers when delegate parameter has some`` (name, method, parameter, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getMemberModifiers project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Parameters)
            |> Seq.filter (fun c -> c.Name = parameter)
        
        <@ actual |> Seq.forall (fun c -> c.Modifier = Some expected) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithOneParameter", "foo")>]
    let ``Analyze should return no default value when method parameter has none`` (name, method, parameter, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Parameters)
            |> Seq.filter (fun c -> c.Name = parameter)

        <@ actual |> Seq.forall (fun c -> c.DefaultValue = None) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithDefaultParameters", "foo", "1")>]
    let ``Analyze should return default value when method parameter has some`` (name, method, parameter, expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method
            |> Seq.collect (fun c -> c.Parameters)
            |> Seq.filter (fun c -> c.Name = parameter)
        
        <@ actual |> Seq.forall (fun c -> c.DefaultValue = Some expected) @>

    [<Theory>]
    [<ProjectData("Methods", "TypeWithMethods", "WithoutParameters", "void")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithReturnType", "Int32")>]
    [<ProjectData("Methods", "TypeWithMethods", "WithGenericReturnType`1", "T")>]
    let ``Analyze should return expected delegate return type`` (name, method, expected, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getType project.Language expected
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library
            |> TestHelper.getMethod method

        test <@ actual |> Seq.forall (fun c -> c.ReturnType = expected) @>