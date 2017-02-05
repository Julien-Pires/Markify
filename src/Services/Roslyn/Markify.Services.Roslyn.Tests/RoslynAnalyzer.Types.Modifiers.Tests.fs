namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesModifiersTests =
    open System
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote
    open Markify.Domain.Ide

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "PublicType", "public")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "InternalType", "internal")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.ProtectedType", "protected")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.PrivateType", "private")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.ProtectedInternalType", "protected;internal")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.CSharp, "ParentType.InternalProtectedType", "protected;internal")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "PublicType", "Public")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "InternalType", "Friend")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.ProtectedType", "Protected")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.PrivateType", "Private")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.ProtectedInternalType", "Protected;Friend")>]
    [<MultiProjectData("TypesInformations/AllTypesModifiers", ProjectLanguage.VisualBasic, "ParentType.InternalProtectedType", "Protected;Friend")>]
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
    [<MultiProjectData("TypesInformations/ContainerModifiers", ProjectLanguage.CSharp, "PartialType", "partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.CSharp, "SealedType", "sealed")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.CSharp, "AbstractType", "abstract")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.CSharp, "StaticType", "static")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.CSharp, "AbstractPartialType", "abstract;partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.CSharp, "SealedPartialType", "sealed;partial")>]
    [<MultiProjectData("TypesInformations/ContainerModifiers", ProjectLanguage.VisualBasic, "PartialType", "Partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "SealedType", "NotInheritable")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "AbstractType", "MustInherit")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "StaticType", "Static")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "AbstractPartialType", "MustInherit;Partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "SealedPartialType", "NotInheritable;Partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "PartialAbstractType", "MustInherit;Partial")>]
    [<MultiProjectData("TypesInformations/ClassModifiers", ProjectLanguage.VisualBasic, "PartialSealedType", "NotInheritable;Partial")>]
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