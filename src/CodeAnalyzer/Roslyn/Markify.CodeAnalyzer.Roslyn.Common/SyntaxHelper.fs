namespace Markify.CodeAnalyzer.Roslyn.Common

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

    let findDefinitionNodes (isVisitable : SyntaxNode -> SyntaxNode option) (isDefinition : SyntaxNode -> SyntaxNode option) (node : SyntaxNode) =
        let rec loop acc nodes =
            let definitions = 
                nodes
                |> Seq.choose isDefinition
                |> Seq.append acc
            let visitables =
                nodes
                |> Seq.choose isVisitable
                |> Seq.collect (fun c -> c.ChildNodes())
            match Seq.isEmpty visitables with
            | false -> loop definitions visitables
            | true -> definitions
        loop Seq.empty (node.ChildNodes())