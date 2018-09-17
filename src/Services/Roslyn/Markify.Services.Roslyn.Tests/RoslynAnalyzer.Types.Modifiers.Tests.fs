namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesModifiersTests =
    open System
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("AccessModifiers", "PublicType", "public")>]
    [<ProjectData("AccessModifiers", "InternalType", "internal")>]
    [<ProjectData("AccessModifiers", "ParentType.ProtectedType", "protected")>]
    [<ProjectData("AccessModifiers", "ParentType.PrivateType", "private")>]
    [<ProjectData("AccessModifiers", "ParentType.ProtectedInternalType", "protected;internal")>]
    [<ProjectData("AccessModifiers", "ParentType.InternalProtectedType", "protected;internal")>]
    let ``Analyze should return definition with access modifiers when type has some`` (name, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiersOld modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinitions name library 
            |> Seq.map (fun c -> Set c.Identity.AccessModifiers)

        test <@ actual |> Seq.forall ((Set.isSubset)expected) @>

    [<Theory>]
    [<ProjectData("Modifiers", "PartialType", "Class;Struct;Interface", "partial")>]
    [<ProjectData("Modifiers", "SealedType", "Class", "sealed")>]
    [<ProjectData("Modifiers", "AbstractType", "Class", "abstract")>]
    [<ProjectData("Modifiers", "StaticType", "Class", "static")>]
    [<ProjectData("Modifiers", "AbstractPartialType", "Class", "abstract;partial")>]
    [<ProjectData("Modifiers", "SealedPartialType", "Class", "sealed;partial")>]
    let ``Analyze should return definition with modifiers when type has some`` (name, namespaces : string, modifiers : string, sut : RoslynAnalyzer, project) =
        let expected = TestHelper.getModifiersOld modifiers project.Language
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.filterDefinitions name (namespaces.Split (';')) library 
            |> Seq.map (fun c -> Set c.Identity.Modifiers)

        test <@ actual |> Seq.forall ((Set.isSubset) expected) @>