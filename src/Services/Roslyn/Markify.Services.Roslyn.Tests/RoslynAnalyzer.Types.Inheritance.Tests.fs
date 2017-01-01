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
    [<MultiProjectData("ContainerTypesInheritance", ProjectLanguage.CSharp, "ImplementInterfaceType", "IDisposable")>]
    [<MultiProjectData("ContainerTypesInheritance", ProjectLanguage.CSharp, "ImplementGenericInterfaceType", "IList<String>")>]
    [<MultiProjectData("ClassTypeInheritance", ProjectLanguage.CSharp, "InheritType", "Exception")>]
    [<MultiProjectData("ClassTypeInheritance", ProjectLanguage.CSharp, "MixedInheritanceType", "Exception;IDisposable")>]
    [<MultiProjectData("EnumTypeInheritance", ProjectLanguage.CSharp, "InheritType", "int")>]
    [<MultiProjectData("ContainerTypesInheritance", ProjectLanguage.VisualBasic, "ImplementInterfaceType", "IDisposable")>]
    [<MultiProjectData("ContainerTypesInheritance", ProjectLanguage.VisualBasic, "ImplementGenericInterfaceType", "IList(Of String)")>]
    [<MultiProjectData("ClassTypeInheritance", ProjectLanguage.VisualBasic, "InheritType", "Exception")>]
    [<MultiProjectData("ClassTypeInheritance", ProjectLanguage.VisualBasic, "MixedInheritanceType", "Exception;IDisposable")>]
    [<MultiProjectData("EnumTypeInheritance", ProjectLanguage.VisualBasic, "InheritType", "Integer")>]
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