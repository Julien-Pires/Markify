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

    let testRepeat setup values tests =
        tests
        |> Seq.map (fun (name, c) ->
            values |> Seq.map (fun value -> Tests.testCase (sprintf "%s - (%A)" name value) (setup value c)))
        |> Seq.collect id

    let testTheory values name test = 
        values |> Seq.map (fun c -> (sprintf "%s (%A)" name c, test c))

    let withProject name language f () = 
        f <|| (ProjectBuilder.create name language, (RoslynAnalyzer(modules) :> IProjectAnalyzer))