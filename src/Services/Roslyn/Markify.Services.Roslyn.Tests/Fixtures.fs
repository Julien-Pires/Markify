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
    
    let buildProjects contents =
        contents
        |> Seq.map (fun c -> 
            let language, content = c
            content |> Seq.map (fun d -> {
                new IProjectContent with
                member __.Content = d
                member __.Language = language
            }))
        |> Seq.map (fun c -> { 
            Name = "Project"
            Content = c |> Seq.toList })

    let withSut f () = f <| (RoslynAnalyzer(modules) :> IProjectAnalyzer)
    
    let withProjects contents f =
        let projects = buildProjects contents
        let sut = RoslynAnalyzer(modules) :> IProjectAnalyzer
        projects |> Seq.map (fun c -> f sut c)