namespace Markify.Services.Roslyn

open System
open System.IO
open Markify.Core.FSharp.IO
open Markify.Core.FSharp.Builders
open Markify.Domain.Compiler
open Markify.Domain.Ide
open Markify.Services.Roslyn.Common
open Markify.Core.FSharp

type RoslynAnalyzer (languages : ILanguageModule seq) =
    let getLanguage languageModule =
        let languageType = languageModule.GetType()
        let attr = languageType.GetCustomAttributes(typeof<LanguageAttribute>, false)   
        match attr |> Seq.tryHead with
        | None -> ProjectLanguage.Unsupported
        | Some x -> (x :?> LanguageAttribute).Language

    let languageModules =
        languages
        |> Seq.fold (fun acc c ->
            match getLanguage c with
            | ProjectLanguage.Unsupported -> acc
            | x -> (x, c)::acc) []
        |> Map.ofList

    let readFile file =
        match IO.readFile file with
        | Success x -> x
        | _ -> ""

    let merge types newTypes languageSyntax = MapBuilder(){
        yield! MissingTypeMerger.merge types newTypes
        yield! PartialMerger.merge types newTypes languageSyntax
        yield! types
    }

    let analyzeSources (files : Uri seq) (analyzer : ILanguageAnalyzer) (languageSyntax : ILanguageSyntax) =
        let types, namespaces =            
            Seq.fold (fun acc (c : Uri) ->
                let types, namespaces = acc
                let content = analyzer.Analyze <| readFile c.AbsolutePath
                (   merge types content.Types languageSyntax,
                    content.Namespaces |> List.append namespaces)) (Map.empty, []) files
        (   types |> Seq.map (fun d -> d.Value) |> Seq.toList,
            namespaces |> List.distinct )    

    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : AssemblyDefinition =
            let languageModule = languageModules.TryFind project.Language
            let types, namespaces =
                match languageModule with
                | None -> ([],[])
                | Some x -> analyzeSources project.Files x.Analyzer x.Syntax
            {   Project = project.Name
                Namespaces = namespaces
                Types = types }