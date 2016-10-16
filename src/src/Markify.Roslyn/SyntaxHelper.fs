namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

module SyntaxHelper =
    let getName (identifier : SyntaxToken) parametersCount =
        match parametersCount with
        | 0 -> identifier.Text
        | x -> sprintf "%s`%i" identifier.Text x
    
    let getParentName (node : SyntaxNode) (getIdentifier : SyntaxNode -> SyntaxToken option) =
        let rec loop (parentNode : SyntaxNode) acc =
            let identifier = getIdentifier parentNode
            match identifier with
            | Some x ->
                let name = 
                    match acc with
                    | "" -> sprintf "%s" x.Text
                    | _ -> sprintf "%s.%s" x.Text acc
                loop parentNode.Parent name
            | None -> acc
        let result = loop node.Parent ""
        match result with
        | "" -> None
        | _ -> Some result

    let getNamespaceName (node : SyntaxNode) (getNamespaceName : SyntaxNode -> SyntaxNode option) =
        let rec loop parentNode =
            let name = getNamespaceName parentNode
            match name with
            | Some x -> Some <| x.ToString()
            | None ->
                match parentNode with
                | null -> None
                | _ -> loop parentNode.Parent
        loop node.Parent