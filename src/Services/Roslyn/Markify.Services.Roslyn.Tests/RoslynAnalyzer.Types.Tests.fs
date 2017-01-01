namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp)>]
    [<MultiProjectData("EmptySourceSamples", ProjectLanguage.CSharp)>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic)>]
    [<MultiProjectData("EmptySourceSamples", ProjectLanguage.VisualBasic)>]
    let ``Analyze should return expected types count`` (sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = 
            projects
            |> Seq.map (fun c -> c.Count) 
            |> Seq.toList
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                (library.Types |> Seq.length)::acc) []
            |> List.rev

        test <@ actual = expected @>

    [<Theory>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.CSharp, "FooType")>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.CSharp, "ParentType.NestedType")>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.CSharp, "FooNamespace.InNamespaceType")>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.VisualBasic, "FooType")>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.VisualBasic, "ParentType.NestedType")>]
    [<MultiProjectData("AllTypesDuplicate", ProjectLanguage.VisualBasic, "FooNamespace.InNamespaceType")>]
    let ``Analyze should return no duplicate types`` (fullname, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let types =
                    library.Types
                    |> Seq.filter (fun d -> getFullname d.Identity = fullname)
                types::acc) []

        test <@ actual |> Seq.forall (fun c -> c |> Seq.length = 1) @>

    [<Theory>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "NestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "InNamespaceType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "DeeperNestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "NestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "InNamespaceType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "DeeperNestedType")>]
    let ``Analyze should return type with correct name`` (name, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let types =
                    library.Types
                    |> Seq.filter (fun d -> d.Identity.Name = name)
                types::acc) []
                
        test <@ actual |> List.forall (fun c -> c |> Seq.length >= 1) @>
    
    [<Theory>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "ParentType.NestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "ParentType.AnotherNestedType.DeeperNestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace.InNamespaceType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.CSharp, "FooNamespace.BarNamespace.ParentType.NestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "ParentType.NestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "ParentType.AnotherNestedType.DeeperNestedType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace.InNamespaceType")>]
    [<MultiProjectData("AllTypesSamples", ProjectLanguage.VisualBasic, "FooNamespace.BarNamespace.ParentType.NestedType")>]
    let ``Analyze should return types with correct fullname`` (fullname, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let types = 
                    library.Types
                    |> Seq.filter (fun d -> getFullname d.Identity = fullname)
                types::acc) []

        test <@ actual |> List.forall (fun c -> c |> Seq.length = 1) @>

    [<Theory>]
    [<MultiProjectData("InvalidFileProject", ProjectLanguage.Unsupported, 0)>]
    let ``Analyze should return nothing when file does not exists or is not supported`` (expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                library.Types::acc) []

        test <@ actual |> List.forall (fun c -> c |> Seq.length = expected) @>