namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Core.Builders
open Markify.Core.Analyzers
open Markify.Models.Definitions
open Markify.Models.IDE

type RoslynAnalyzer (languageAnalyzers : ((string -> SourceContent) * string seq) seq) =
    let languageAnalyzers =
        languageAnalyzers
        |> Seq.fold (fun acc c ->
            let analyzer, extensions = c
            extensions
            |> Seq.map (fun d -> (d.ToLower(), analyzer))
            |> Seq.append acc) Seq.empty
        |> Map.ofSeq

    let readFile file =
        match IO.readFile file with
        | Success x -> x
        | _ -> ""

    let findAnalyzer file =
        let ext =
            match Path.GetExtension (file) with
            | "" -> String.Empty
            | x -> x.Substring(1)
        languageAnalyzers.TryFind <| ext.ToLower()

    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : LibraryDefinition =
            let definitions =
                ({ SourceContent.Namespaces = []; Types = [] }, project.Files)
                ||> Seq.fold (fun acc c ->
                    let analyzer = findAnalyzer c.AbsolutePath
                    match analyzer with
                    | None -> acc
                    | Some x ->
                        let content = x <| readFile c.AbsolutePath
                        { acc with
                            Namespaces = List.append acc.Namespaces content.Namespaces
                            Types = List.append acc.Types content.Types })
                |> fun c ->
                    { c with
                        Namespaces = c.Namespaces |> List.distinctBy (fun c -> c.Name) 
                        Types = c.Types |> List.distinctBy (fun c -> c.Identity) }
            {   Project = project.Name
                Namespaces = definitions.Namespaces
                Types = definitions.Types }