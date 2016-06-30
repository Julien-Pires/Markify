namespace Markify.Roslyn

open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

module TypeExtension =
    let getName node =
        match node with
        | Type c ->
            Some c.Name
//            let parametersLength =
//                match node with
//                | GenericNode genInfo -> genInfo.Parameters |> Seq.length
//                | _ -> 0
//            match parametersLength with
//            | 0 -> Some info.Name
//            | _ -> Some (sprintf "%s`%i" info.Name parametersLength)
        | Namespace c -> Some c.Name
        | _ -> None

    let getFullname node : DefinitionFullname =
        let rec loopParentNode innerNode acc =
            match innerNode with
            | Type c ->
                let name = (getName innerNode).Value
                match acc with
                | "" -> sprintf "%s" name
                | _ -> sprintf "%s.%s" name acc
                |> loopParentNode c.Parent.Value
            | Namespace c -> sprintf "%s.%s" (getName innerNode).Value acc
            | Other c -> loopParentNode c.Parent.Value acc
            | NoNode _ -> acc

        loopParentNode node ""