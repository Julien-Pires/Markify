namespace Markify.Services.Roslyn.Tests

open Expecto
open Markify.Domain.Compiler
open Markify.Services.Roslyn
open Markify.Services.Roslyn.Common
open Markify.Services.Roslyn.Csharp
open Markify.Services.Roslyn.VisualBasic

[<RequireQualifiedAccess>]
module Seq =
    let forAllStrict predicate source = 
        match source |> Seq.isEmpty with
        | false -> Seq.forall predicate source
        | true -> false

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

    let testRepeat repeater name test =
        repeater test
        |> Seq.mapi (fun index c -> testCase (sprintf "%s #%i" name index) c)

    let testRepeatParameterized name setup test =
        setup
        |> Seq.collect (fun (repeater, parameters) ->
            repeater test |> Seq.mapi (fun index c -> testCase (sprintf "%s (%A) #%i" name parameters index) (c parameters)))