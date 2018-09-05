namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Services.Roslyn
open Markify.Services.Roslyn.Common
open Markify.Services.Roslyn.Csharp
open Markify.Services.Roslyn.VisualBasic

module Fixtures =
    let modules = [
        CSharpModule() :> ILanguageModule
        VisualBasicModule() :> ILanguageModule ]

    let testRepeat (setup: 'a -> 'b) (tests: (string * 'a) seq) =
        tests
        |> Seq.map (fun (name, c) -> setup c |> Seq.mapi (fun index d -> (sprintf "%s - %i" name index, d)))
        |> Seq.collect id

    let testTheory values tests = 
        tests
        |> Seq.map (fun (name, c) -> 
            values |> Seq.map (fun d -> (sprintf "%s (%A)" name d, c d)))
        |> Seq.collect id

    let withRoslynAnalyzer f () = f <| (RoslynAnalyzer(modules) :> IProjectAnalyzer)

    let withProject name languages f = 
        languages |> Seq.map (fun c -> f <| ProjectBuilder.create name c)