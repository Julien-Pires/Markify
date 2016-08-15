namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

module TypeExtension =
    let getName node =
        match node with
        | Type x ->
            match x.Parameters |> Seq.length with
            | 0 -> x.Name
            | w -> sprintf "%s`%i" x.Name w
        | Namespace x -> x.Name
        | _ -> ""
    
    let getParentName (node : TypeNode) =
        let rec loopParentNode parentNode acc =
            match parentNode with
            | Type x -> 
                let name = 
                    match acc with
                    | "" -> sprintf "%s" x.Name
                    | _ -> sprintf "%s.%s" x.Name acc
                loopParentNode x.Parent.Value name
            | _ -> acc
        let parent =
            match node.Parent.Value with
            | Type _ -> Some node.Parent.Value
            | _ -> None
        match parent with
        | Some x -> Some <| loopParentNode x ""
        | None -> None

    let getNamespaceName node =
        let rec loopParentNode n =
            match n with
            | Type x -> loopParentNode x.Parent.Value
            | Namespace x -> Some x.Name
            | _ -> None

        loopParentNode node

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
            let generic = {
                Identity = c.Name
                Modifier = c.Modifier
                Constraints = constraints}
            generic)