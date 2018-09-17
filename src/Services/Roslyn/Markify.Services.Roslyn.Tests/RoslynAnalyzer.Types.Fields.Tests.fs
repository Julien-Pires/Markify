namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesFieldsTests =
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("Fields", "TypeWithNoField")>]
    let ``Analyze should return definition with no fields when type has none`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getFields
        
        test <@ actual |> Seq.forall (Seq.isEmpty) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields")>]
    let ``Analyze should return definition with fields when type has some`` (name, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getFields
        
        test <@ actual |> Seq.forall (Seq.isEmpty >> not) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "Field")>]
    let ``Analyze should return expected field name`` (name, field, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map DefinitionsHelper.getFields

        test <@ actual |> Seq.map (fun c -> c |> Seq.tryFind (fun d -> d.Name = field))
                       |> Seq.forall (Option.isSome) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "Field", "private")>]
    let ``Analyze should return default access modifier when field has none`` (name, field, modifier, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelper.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field

        test <@ actual |> Seq.forall (fun c -> c.AccessModifiers |> Seq.contains expected) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "Class;Struct", "PublicField", "public")>]
    [<ProjectData("Fields", "TypeWithFields", "Class;Struct", "InternalField", "internal")>]
    [<ProjectData("Fields", "TypeWithFields", "Class;Struct", "PrivateField", "private")>]
    [<ProjectData("Fields", "TypeWithFields", "Class", "ProtectedInternalField", "protected;internal")>]
    [<ProjectData("Fields", "TypeWithFields", "Class", "ProtectedField", "protected")>]
    let ``Analyze should return field access modifiers when field has some`` (name, namespaces : string, field, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiersOld modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split(';')) library
            |> TestHelper.getField field

        test <@ actual |> Seq.map (fun c -> Set c.AccessModifiers)
                       |> Seq.forall (Set.isSubset expected) @>

    [<Theory>]
    [<SingleLanguageProjectData("Fields", ProjectLanguage.CSharp, "TypeWithFields", "Field")>]
    let ``Analyze should return no modifiers when field has none`` (name, field, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field

        test <@ actual |> Seq.forall (fun c -> c.Modifiers |> Seq.isEmpty) @>
    
    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "StaticField", "static")>]
    [<ProjectData("Fields", "TypeWithFields", "ConstField", "const")>]
    [<ProjectData("Fields", "TypeWithFields", "ReadOnlyField", "readonly")>]
    [<ProjectData("Fields", "TypeWithFields", "StaticReadOnlyField", "static;readonly")>]
    [<SingleLanguageProjectData("Fields", ProjectLanguage.VisualBasic, "TypeWithFields", "Field", "Dim")>]
    let ``Analyze should return modifiers when field has some`` (name, field, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getMemberModifiers modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field

        test <@ actual |> Seq.map (fun c -> Set c.Modifiers)
                       |> Seq.forall (Set.isSubset expected) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "Field", "Int32")>]
    let ``Analyze should return field type`` (name, field, expected, sut : RoslynAnalyzer, project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field
        
        test <@ actual |> Seq.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "Field")>]
    let ``Analyze should return no default value when field has none`` (name, field, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field

        test <@ actual |> Seq.forall (fun c -> c.DefaultValue.IsNone) @>
    
    [<Theory>]
    [<ProjectData("Fields", "TypeWithFields", "ConstField", "1")>]
    let ``Analyze should return default value when field has some`` (name, field, value, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library
            |> TestHelper.getField field

        test <@ actual |> Seq.forall (fun c -> c.DefaultValue = Some value) @>