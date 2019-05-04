﻿namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn.Csharp
open Markify.CodeAnalyzer.Roslyn.VisualBasic

module Fixtures =
    let analyzers = [
        CSharpAnalyzer() :> ISourceAnalyzer
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

    let withSut f () = f <| (ProjectAnalyzer(analyzers) :> IProjectAnalyzer)
    
    let withProjects contents f =
        let projects = buildProjects contents
        let sut = ProjectAnalyzer(analyzers) :> IProjectAnalyzer
        projects |> Seq.map (fun c -> f sut c)