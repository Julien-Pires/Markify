namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Core.Builders

type LanguageConverter (nodeHelper : NodeHelper, extensions : string list) =
    let nodeHelper = NodeFactory(nodeHelper)

    member this.Extensions with get() = extensions

    member this.Convert(source) =
        nodeHelper.GetNodes(source)

type SourceConverter (readers : LanguageConverter seq) =
    let readers =
        readers
        |> Seq.fold (fun acc c ->
            let readerExtensions =
                c.Extensions
                |> List.map (fun d -> (d.ToLower(), c))
            seq { yield! acc; yield! readerExtensions }) Seq.empty
        |> Map.ofSeq

    let readFile file =
        match IO.readFile file with
        | Success x -> x
        | _ -> ""

    member this.Convert file =
        let ext = 
            match Path.GetExtension (file) with
            | x when x.StartsWith(".") -> x.Substring(1)
            | x -> x
        let reader = readers.TryFind <| ext.ToLower()
        match reader with
        | None -> []
        | Some x ->
            let fileContent = readFile file
            x.Convert fileContent