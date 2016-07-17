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

    let getGenericParameters (node : TypeNode) =
        node.Parameters
        |> Seq.map (fun c ->
            let paramConstraint =
                node.Constraints
                |> Seq.tryPick (fun d -> 
                    match c.Name with
                    | x when d.Name = x -> Some d
                    | _ -> None)
            let constraints =
                match paramConstraint with
                | Some x -> x.Constraints
                | None -> Seq.empty
            let identity = { 
                Fullname = c.Name
                Name = c.Name }
            let generic = {
                Identity = identity
                Modifier = c.Modifier
                Constraints = constraints}
            generic)