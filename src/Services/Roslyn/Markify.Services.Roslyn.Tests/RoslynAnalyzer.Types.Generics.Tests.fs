namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesGenericsTests =
    open System
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote
    open Markify.Domain.Ide

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1", 1)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", 2)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1", 1)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", 2)>]
    let ``Analyze should return correct parameters count when type has some`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                typeDefinition::acc) []

        test <@ actual |> List.forall (fun c -> (c.Identity.Parameters |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2")>]
    let ``Analyze should return correct type name when type has some parameters`` (expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinitions =
                    library.Types
                    |> Seq.filter (fun d -> d.Identity.Name = expected)
                typeDefinitions::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) >= 1) @>

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1", "T")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "T")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "Y")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1", "T")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "T")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "Y")>]
    let ``Analyze should return correct parameter name when type has some`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                let parameters =
                    typeDefinition.Identity.Parameters
                    |> Seq.filter (fun d -> d.Name = expected)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) >= 1) @>
    
    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1", "T", "")>]
    [<MultiProjectData("TypesInformations/GenericVariance", ProjectLanguage.CSharp, "CovariantGenericType`1", "T", "in")>]
    [<MultiProjectData("TypesInformations/GenericVariance", ProjectLanguage.CSharp, "ContravariantGenericType`1", "T", "out")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1", "T", "")>]
    [<MultiProjectData("TypesInformations/GenericVariance", ProjectLanguage.VisualBasic, "CovariantGenericType`1", "T", "In")>]
    [<MultiProjectData("TypesInformations/GenericVariance", ProjectLanguage.VisualBasic, "ContravariantGenericType`1", "T", "Out")>]
    let ``Analyze should return correct parameter modifier`` (name, parameter, modifier, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected =
            match modifier with
            | "" -> None
            | x -> Some x
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                let parameter =
                    typeDefinition.Identity.Parameters
                    |> Seq.find (fun d -> d.Name = parameter)
                parameter::acc) []

        test <@ actual |> List.forall (fun c -> c.Modifier = expected) @>

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1", "T", 0)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "Y", 1)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "T", 2)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1", "T", 0)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "Y", 1)>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "T", 2)>]
    let ``Analyze should return correct parameter constraints count when parameter has some`` (name, parameter, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                let parameter =
                    typeDefinition.Identity.Parameters
                    |> Seq.find (fun d -> d.Name = parameter)
                parameter.Constraints::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "SingleGenericType`1", "T", "")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "Y", "struct")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.CSharp, "MultipleGenericType`2", "T", "class;IList<string>")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "SingleGenericType`1", "T", "")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "Y", "Structure")>]
    [<MultiProjectData("TypesInformations/AllTypesGenerics", ProjectLanguage.VisualBasic, "MultipleGenericType`2", "T", "Class;IList(Of String)")>]
    let ``Analyze should return correct constraint name when parameter has some`` (name, parameter, constraints : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| constraints.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let typeDefinition =
                    library.Types
                    |> Seq.find (fun d -> getFullname d.Identity = name)
                let parameter =
                    typeDefinition.Identity.Parameters
                    |> Seq.find (fun d -> d.Name = parameter)
                let parameterConstraints = Set parameter.Constraints
                parameterConstraints::acc) []

        test <@ actual |> List.forall ((Set.isSubset) expected) @>