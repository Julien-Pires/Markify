namespace Markify.Services.Roslyn.Tests

open System
open Markify.Domain.Ide
open Markify.Domain.Compiler
open Markify.Services.Roslyn
open Xunit
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesPropertiesTests =
    [<Tests>]
    let analyzeTests =
        testList "Analyze" [
            yield! testRepeatOld (withProjectOld "Properties") allLanguages [
                yield "should return no properties when type has none",
                fun project (sut : IProjectAnalyzer) -> 
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies "TypeWithNoProperties"

                    test <@ result |> Seq.map getProperties
                                   |> Seq.forAllStrict Seq.isEmpty @>

                yield "should return properties when type has some",
                fun project (sut : IProjectAnalyzer) -> 
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies "TypeWithProperties"

                    test <@ result |> Seq.map getProperties
                                   |> Seq.forAllStrict (Seq.isEmpty >> not) @>

                yield! testTheory "should return property with valid name when type has some"
                    ["AutoProperty";
                    "ReadOnlyProperty";
                    "WriteOnlyProperty"]
                    (fun property project (sut : IProjectAnalyzer) -> 
                        let assemblies = sut.Analyze project
                        let result = filterTypes assemblies "TypeWithProperties"
                        
                        test <@ result |> Seq.map getProperties
                                       |> Seq.forAllStrict (Seq.exists (fun c -> c.Name = property)) @>)
                
                yield! testTheory "should return default acess modifier when property has no specified modifier"
                    [("TypeWithPublicDefaultModifier", "public");
                    ("TypeWithPrivateDefaultModifier", "private")]
                    (fun parameters project (sut : IProjectAnalyzer) ->
                        let name, modifier = parameters 
                        let expected = getModifier modifier project.Language
                        let assemblies = sut.Analyze project
                        let result = filterTypes assemblies name

                        test <@ result |> Seq.map getProperties
                                       |> Seq.map (Seq.find (fun c -> c.Name = "WithNoModifierProperty"))
                                       |> Seq.forAllStrict (fun c -> Seq.contains expected c.AccessModifiers) @>)
                
                yield! testTheory "should return access modifiers when property has some"
                    [(["Class"; "Struct"; "Interface"], "AutoProperty", ["public"]);
                    (["Class"; "Struct"], "InternalProperty", ["internal"]);
                    (["Class"; "Struct"], "PrivateProperty", ["private"]);
                    (["Class"], "ProtectedInternalProperty", ["protected"; "internal"]);
                    (["Class"], "ProtectedProperty", ["protected"])]
                    (fun parameters project (sut : IProjectAnalyzer) ->
                        let namespaces, property, modifiers = parameters
                        let expected = modifiers |> Seq.map (fun c -> getModifier c project.Language)
                        let assemblies = sut.Analyze project
                        let result =
                            namespaces
                            |> Seq.map (fun c -> filterTypes assemblies (sprintf "%s.TypeWithProperties" c))
                            |> Seq.collect id
                        
                        test <@ result |> Seq.map getProperties
                                       |> Seq.map (Seq.find (fun c -> c.Name = property)) 
                                       |> Seq.forAllStrict (fun c -> Set.isSubset (Set expected) (Set c.AccessModifiers)) @>)

                yield "should return property with no modifiers when property has none",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies "TypeWithProperties"

                    test <@ result |> Seq.map getProperties
                                   |> Seq.map (Seq.filter (fun c -> c.Name = "AutoProperty"))
                                   |> Seq.forAllStrict (Seq.exists (fun c -> Seq.isEmpty c.Modifiers)) @>

                yield! testTheory "should return modifiers when property has some"
                    [(["Class"; "Struct"], "StaticProperty", ["static"]);
                    (["Class"], "SealedProperty", ["sealed"; "override"]);
                    (["Class"], "VirtualProperty", ["virtual"])]
                    (fun parameters project (sut : IProjectAnalyzer) ->
                        let namespaces, property, modifiers = parameters
                        let expected = modifiers |> Seq.map (fun c -> getModifier c project.Language)
                        let assemblies = sut.Analyze project
                        let result =
                            namespaces
                            |> Seq.map (fun c -> filterTypes assemblies (sprintf "%s.TypeWithProperties" c))
                            |> Seq.collect id
                        
                        test <@ result |> Seq.map getProperties
                                       |> Seq.map (Seq.find (fun c -> c.Name = property)) 
                                       |> Seq.forAllStrict (fun c -> Set.isSubset (Set expected) (Set c.Modifiers)) @>)
            ]
        ]
    
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
        let expected = LanguageHelperOld.getModifier project.Language modifier
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
        let expected = LanguageHelperOld.getModifier project.Language modifier
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.filterDefinitions name (namespaces.Split (';')) library
            |> TestHelper.getProperty property

        test <@ actual |> Seq.map (fun c -> c.IsRead.Value.AccessModifiers)
                       |> Seq.forall (fun c -> c |> Seq.contains expected) @>