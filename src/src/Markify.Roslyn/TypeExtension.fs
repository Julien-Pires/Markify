module TypeExtension
    open SyntaxNodeExtension
    open Markify.Models.Definitions

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp

    let accessModifiersList = Set [ 
                                SyntaxKind.PublicKeyword
                                SyntaxKind.InternalKeyword 
                                SyntaxKind.PrivateKeyword
                                SyntaxKind.ProtectedKeyword ]

    let name (node : SyntaxNode) =
        match node with
        | TypeInfo info ->
            let parametersLength = Seq.length info.Parameters
            match parametersLength with
            | 0 -> Some(info.Name)
            | _ -> Some(sprintf "%s`%i" info.Name parametersLength)
        | NamespaceNode n -> Some(n.Name.ToString())
        | _ -> None

    let fullname (node : SyntaxNode) : DefinitionFullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc =
            match innerNode with
            | TypeNode t ->
                match acc with
                | "" -> sprintf "%s" (name innerNode).Value
                | _ -> sprintf "%s.%s" (name innerNode).Value acc
                |> loopParentNode innerNode.Parent
            | NamespaceNode n -> sprintf "%s.%s" (name n).Value acc
            | null -> acc
            | _ -> loopParentNode innerNode.Parent acc

        loopParentNode node ""

    let accessModifiers (node : SyntaxNode) = 
        match node with
        | TypeInfo info ->
            info.Modifiers
            |> Seq.filter (fun c -> Set.contains (c.Kind()) accessModifiersList)
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty<string>

    let additionalModifiers (node : SyntaxNode) = 
        match node with
        | TypeInfo info ->
            info.Modifiers
            |> Seq.filter (fun c -> not (Set.contains (c.Kind()) accessModifiersList))
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty<string>

    let baseTypes (node : SyntaxNode) =
        match node with
        | InheritableType it ->
            match it.BaseList with
            | null -> Seq.empty<string>
            | x -> x.Types |> Seq.map (fun c -> c.Type.ToString())
        | _ -> Seq.empty<string>