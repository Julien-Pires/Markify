namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

module TypeExtension =
    let getName node =
        match node with
        | Type c ->
            match c.Parameters |> Seq.length with
            | 0 -> c.Name
            | x -> sprintf "%s`%i" c.Name x
        | Namespace c -> c.Name
        | _ -> ""

    let getFullname node : DefinitionFullname =
        let rec loopParentNode innerNode acc =
            match innerNode with
            | Type c ->
                let name = getName innerNode
                match acc with
                | "" -> sprintf "%s" name
                | _ -> sprintf "%s.%s" name acc
                |> loopParentNode c.Parent.Value
            | Namespace c -> sprintf "%s.%s" (getName innerNode) acc
            | Other c -> loopParentNode c.Parent.Value acc
            | NoNode _ -> acc

        loopParentNode node ""