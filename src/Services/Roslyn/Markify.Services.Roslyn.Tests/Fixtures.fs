namespace Markify.Services.Roslyn.Tests

open Expecto
open Markify.Domain.Ide
open Markify.Domain.Compiler
open Markify.Services.Roslyn
open Markify.Services.Roslyn.Common
open Markify.Services.Roslyn.Csharp
open Markify.Services.Roslyn.VisualBasic

module Fixtures =
    let CSharp = ProjectLanguage.CSharp
    let VisualBasic = ProjectLanguage.VisualBasic

    let modules = [
        CSharpModule() :> ILanguageModule
        VisualBasicModule() :> ILanguageModule ]

    let testRepeat setup tests =
        tests
        |> Seq.map (fun (name, c) -> 
            setup c |> Seq.mapi (fun index d -> Tests.testCase (sprintf "%s - %i" name index) d))
        |> Seq.collect id

    let testTheory values name test = 
        values 
        |> Seq.map (fun c -> (sprintf "%s (%A)" name c, test c))

    let withProject name languages f = 
        languages 
        |> Seq.map (fun c ->
            fun () -> f <|| (ProjectBuilder.create name c, (RoslynAnalyzer(modules) :> IProjectAnalyzer)))