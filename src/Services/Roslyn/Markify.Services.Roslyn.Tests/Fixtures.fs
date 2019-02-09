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
        |> Seq.map (fun c -> Tests.testCase name c)

    let testRepeatParameterized name setup test =
        setup
        |> Seq.collect (fun (repeater, parameters) ->
            repeater test |> Seq.map (fun c -> Tests.testCase name (c parameters)))

    ////////////////////////////////////////////

    let CSharp = ProjectLanguage.CSharp
    let VisualBasic = ProjectLanguage.VisualBasic
    let allLanguages = [CSharp; VisualBasic]

    let testRepeatOld setup values tests =
        tests
        |> Seq.map (fun (name, c) ->
            values 
            |> Seq.map (fun value -> Tests.testCase (sprintf "%s - %A" name value) (setup value c)))
        |> Seq.collect id

    let testTheory name values test = 
        values 
        |> Seq.map (fun c -> (sprintf "%s / %A" name c, test c))

    let withProjectOld name language f () = 
        f <|| (ProjectBuilder.create name language, (RoslynAnalyzer(modules) :> IProjectAnalyzer))