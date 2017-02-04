namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesInheritanceTests =
    open System
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("TypesInformations/ContainerInheritance", ProjectLanguage.CSharp, "ImplementInterfaceType", "IDisposable")>]
    [<MultiProjectData("TypesInformations/ContainerInheritance", ProjectLanguage.CSharp, "ImplementGenericInterfaceType", "IList<String>")>]
    [<MultiProjectData("TypesInformations/ClassInheritance", ProjectLanguage.CSharp, "InheritType", "Exception")>]
    [<MultiProjectData("TypesInformations/ClassInheritance", ProjectLanguage.CSharp, "MixedInheritanceType", "Exception;IDisposable")>]
    [<MultiProjectData("TypesInformations/EnumInheritance", ProjectLanguage.CSharp, "InheritType", "int")>]
    [<MultiProjectData("TypesInformations/ContainerInheritance", ProjectLanguage.VisualBasic, "ImplementInterfaceType", "IDisposable")>]
    [<MultiProjectData("TypesInformations/ContainerInheritance", ProjectLanguage.VisualBasic, "ImplementGenericInterfaceType", "IList(Of String)")>]
    [<MultiProjectData("TypesInformations/ClassInheritance", ProjectLanguage.VisualBasic, "InheritType", "Exception")>]
    [<MultiProjectData("TypesInformations/ClassInheritance", ProjectLanguage.VisualBasic, "MixedInheritanceType", "Exception;IDisposable")>]
    [<MultiProjectData("TypesInformations/EnumInheritance", ProjectLanguage.VisualBasic, "InheritType", "Integer")>]
    let ``Analyze should return base types when type has some`` (fullname, types : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| types.Split(';')
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition = 
                    library.Types
                    |> Seq.find (fun c -> getFullname c.Identity = fullname)
                typeDefinition::acc) []
            |> List.map (fun c -> Set c.Identity.BaseTypes)        

        test <@ actual |> List.forall ((Set.isSubset) expected) @>