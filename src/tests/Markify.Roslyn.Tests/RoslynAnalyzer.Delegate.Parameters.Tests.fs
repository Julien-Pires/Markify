namespace Markify.Roslyn.Tests

module RoslynAnalyzerDelegateParametersTests =
    open System
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Models.Definitions
    open Markify.Core.Analyzers
    open Attributes
    open Swensen.Unquote
    open Xunit

    let getParameters = function
        | Delegate x -> x.Parameters
        | _ -> Seq.empty

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithoutParameters", 0)>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithOneParameter", 1)>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithMultipleParameters", 2)>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithoutParameters", 0)>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithOneParameter", 1)>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithMultipleParameters", 2)>]
    let ``Analyze should return expected delegate parameters count`` (delegateName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun c -> c.Identity.Name = delegateName)
                    |> getParameters
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithOneParameter", "foo")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithParametersModifiers", "bar")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithOneParameter", "foo")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithParametersModifiers", "bar")>]
    let ``Analyze should return expected delegate parameter name`` (delegateName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun c -> c.Identity.Name = delegateName)
                    |> getParameters
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c |> Seq.exists(fun d -> d.Name = expected)) @>

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithOneParameter", "foo", "int")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithGenericParameters`1", "foo", "T")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithGenericParameters`1", "bar", "T[]")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithOneParameter", "foo", "Integer")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithGenericParameters`1", "foo", "T")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithGenericParameters`1", "bar", "T()")>]
    let ``Analyze should return expected delegate parameter type`` (delegateName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = delegateName)
                    |> getParameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c.Type = expected) @>
    
    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithOneParameter", "foo")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithOneParameter", "foo")>]
    let ``Analyze should return no modifier when delegate parameter has none`` (delegateName, parameterName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = delegateName)
                    |> getParameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c.Modifier = None) @>

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithParametersModifiers", "foo", "ref")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithParametersModifiers", "bar", "out")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithParametersModifiers", "foo", "ByRef")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithParametersModifiers", "bar", "ByVal")>]
    let ``Analyze should return expected delegate parameter modifiers`` (delegateName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = delegateName)
                    |> getParameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c.Modifier = Some expected) @>

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithOneParameter", "foo")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithOneParameter", "foo")>]
    let ``Analyze should return no default value when delegate parameter has none`` (delegateName, parameterName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = delegateName)
                    |> getParameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c.DefaultValue = None) @>

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp, "WithDefaultParameters", "foo", "1")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic, "WithDefaultParameters", "foo", "1")>]
    let ``Analyze should return expected delegate parameter default value`` (delegateName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameters =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = delegateName)
                    |> getParameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> c.DefaultValue = Some expected) @>