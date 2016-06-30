namespace Markify.Roslyn

open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

module TypeExtension =
    let (|TypeNode|_|) (node : TypeNode) =
        match node.Kind with
        | Type _ -> Some node
        | _ -> None

    let (|NamespaceNode|_|) (node : TypeNode) =
        match node.Kind with
        | Namespace _ -> Some node
        | _ -> None

    let getName (node : TypeNode) =
        match node with
        | TypeNode _ ->
            Some node.Name
//            let parametersLength =
//                match node with
//                | GenericNode genInfo -> genInfo.Parameters |> Seq.length
//                | _ -> 0
//            match parametersLength with
//            | 0 -> Some info.Name
//            | _ -> Some (sprintf "%s`%i" info.Name parametersLength)
        | NamespaceNode _ -> Some node.Name
        | _ -> None

    let getFullname (node : SyntaxNode) : DefinitionFullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc =
            match innerNode with
            | TypeNode _ ->
                let name = getName innerNode
                match acc with
                | "" -> sprintf "%s" name.Value
                | _ -> sprintf "%s.%s" name.Value acc
                |> loopParentNode innerNode.Parent
            | NamespaceNode n -> sprintf "%s.%s" (getName n).Value acc
            | null -> acc
            | _ -> loopParentNode innerNode.Parent acc

        loopParentNode node ""