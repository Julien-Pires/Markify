namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Core.Builders

type SourceConverter (readers : (NodeHelper * string list) list) =
    let readers =
        readers
        |> List.fold (fun acc c ->
            let helper, extensions = c
            extensions
            |> List.map (fun d -> (d.ToLower(), NodeFactory(helper)))
            |> List.append acc) []
        |> Map.ofList

    let readFile file =
        match IO.readFile file with
        | Success x -> x
        | _ -> ""

    member this.Convert file =
        let ext = 
            match Path.GetExtension (file) with
            | "" -> String.Empty
            | x -> x.Substring(1)
        let reader = readers.TryFind <| ext.ToLower()
        match reader with
        | None -> []
        | Some x ->
            let fileContent = readFile file
            x.GetNodes fileContent