module TypeExtension
    open SyntaxNodeExtension
    open Markify.Models.Definitions

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax 

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

    let createGenericDefinition (parameter : TypeParameterSyntax) (typeConstraints : TypeParameterConstraintClauseSyntax seq) =
        let name = parameter.Identifier.ToString()
        let identity = { Fullname = name; Name = name }
        let constraints =
            typeConstraints
            |> Seq.tryPick (fun c ->
                match c.Name.ToString() with
                | x when name = x -> Some c
                | _ -> None
            )
        let parameter = {
            Identity = identity;
            Modifier = parameter.VarianceKeyword.Text;
            Constraints = 
                match constraints with
                | Some x -> ([], x.Constraints) ||> Seq.fold (fun acc c -> c.ToString()::acc) |> List.toSeq
                | _ -> Seq.empty
        }

        parameter

    let getGenericParameters (node : SyntaxNode) =
        match node with
        | TypeInfo info ->
            info.Parameters
            |> Seq.map (fun c -> createGenericDefinition c info.Constraints)
        | _ -> Seq.empty

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