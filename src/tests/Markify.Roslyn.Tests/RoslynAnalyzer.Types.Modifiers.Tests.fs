namespace Markify.Roslyn.Tests

module RoslynAnalyzerTypesModifiersTests =
    open System
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Models.Definitions
    open Markify.Core.Analyzers
    open Attributes
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "PublicType", "public")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "InternalType", "internal")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.ProtectedType", "protected")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.PrivateType", "private")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.ProtectedInternalType", "protected;internal")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.InternalProtectedType", "protected;internal")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "PublicType", "Public")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "InternalType", "Friend")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.ProtectedType", "Protected")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.PrivateType", "Private")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.ProtectedInternalType", "Protected;Friend")>]
    [<MultiProjectData("AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.InternalProtectedType", "Protected;Friend")>]
    let ``Analyze should return all access modifiers when type has some`` (name, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split (';')
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                typeDefinition::acc) []
            |> List.map (fun c -> Set c.Identity.AccessModifiers)

        test <@ actual |> List.forall ((Set.isSubset)expected) @>

    [<Theory>]
    [<MultiProjectData("ContainerTypesModifiers", ProjectLanguage.CSharp, "PartialType", "partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.CSharp, "SealedType", "sealed")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.CSharp, "AbstractType", "abstract")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.CSharp, "StaticType", "static")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.CSharp, "AbstractPartialType", "abstract;partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.CSharp, "SealedPartialType", "sealed;partial")>]
    [<MultiProjectData("ContainerTypesModifiers", ProjectLanguage.VisualBasic, "PartialType", "Partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "SealedType", "NotInheritable")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "AbstractType", "MustInherit")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "StaticType", "Static")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "AbstractPartialType", "MustInherit;Partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "SealedPartialType", "NotInheritable;Partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "PartialAbstractType", "MustInherit;Partial")>]
    [<MultiProjectData("ClassTypeModifiers", ProjectLanguage.VisualBasic, "PartialSealedType", "NotInheritable;Partial")>]
    let ``Analyze should return all modifiers when type has some`` (name, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split (';')
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                typeDefinition::acc) []
            |> List.map (fun c -> Set c.Identity.Modifiers)

        test <@ actual |> List.forall ((Set.isSubset) expected) @>