namespace Markify.Roslyn.Tests

module RoslynAnalyzerDelegateReturnTypeTests =
    open System
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Models.Definitions
    open Markify.Core.Analyzers
    open Attributes
    open Swensen.Unquote
    open Xunit

    let getReturnType = function
        | Delegate x -> Some x.ReturnType
        | _ -> None

    [<Theory>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.CSharp,"WithoutParameters", "void")>]
    [<MultiProjectData("DelegateParameters", ProjectLanguage.VisualBasic,"WithoutParameters", "Void")>]
    let ``Analyze should return expected delegate return type`` (typename, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let returnType = 
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = typename)
                    |> getReturnType
                returnType::acc) []

        test <@ actual |> List.forall ((=) (Some expected)) @>