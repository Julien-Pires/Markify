module TypeExtension
    open Representation
    open SyntaxNodeExtension

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

    let fullname (node : SyntaxNode) : Fullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc: Fullname =
            match innerNode with
            | TypeNode t -> 
                acc
                |> Seq.append (Seq.singleton (name innerNode).Value)
                |> loopParentNode innerNode.Parent
            | NamespaceNode n -> 
                acc
                |> Seq.append (Seq.singleton (name n).Value)
            | null -> acc
            | _ -> loopParentNode innerNode.Parent acc

        loopParentNode node Seq.empty<string>

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