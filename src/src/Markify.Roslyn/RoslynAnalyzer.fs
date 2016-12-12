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
            let typeDefinitions, namespaceDefinitions =
                project.Files
                |> Seq.fold (fun acc c ->
                    let analyzer = findAnalyzer c.AbsolutePath
                    match analyzer with
                    | None -> acc
                    | Some x ->
                        let { SourceContent.Types=types; Namespaces=namespaces } = x <| readFile c.AbsolutePath
                        let accTypes, accNamespaces = acc
                        (   List.append types accTypes,
                            List.append namespaces accNamespaces)) ([],[])
                |> fun (types, namespaces) ->
                    let cleanTypes =
                        types
                        |> List.groupBy (fun c -> getFullname c.Identity)
                        |> List.fold (fun acc c ->
                            let _, typesList = c
                            let definition = PartialMerger.mergePartialTypes typesList
                            definition::acc) []
                    let cleanNamespaces = 
                        namespaces 
                        |> List.distinctBy (fun c -> c.Name)
                    (cleanTypes, cleanNamespaces)
            {   Project = project.Name
                Namespaces = namespaceDefinitions
                Types = typeDefinitions }