module TypeExtension
    open SyntaxNodeExtension
    open Markify.Models.Definitions

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax 

    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.InternalKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

    let getTypeKind (node : SyntaxNode) =
        match node with
        | ClassNode _ -> StructureKind.Class
        | InterfaceNode _ -> StructureKind.Interface
        | StructNode _ -> StructureKind.Struct
        | EnumNode _ -> StructureKind.Enum
        | DelegateNode _ -> StructureKind.Delegate
        | _ -> StructureKind.Unknown

    let getName (node : SyntaxNode) =
        match node with
        | TypeNode info ->
            let parametersLength =
                match node with
                | GenericNode genInfo -> genInfo.Parameters |> Seq.length
                | _ -> 0
            match parametersLength with
            | 0 -> Some info.Name
            | _ -> Some (sprintf "%s`%i" info.Name parametersLength)
        | NamespaceNode n -> Some (n.Name.ToString())
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

    let createGenericDefinition (typeConstraints : TypeParameterConstraintClauseSyntax seq) (parameter : TypeParameterSyntax) =
        let name = parameter.Identifier.ToString()
        let identity = { Fullname = name; Name = name }
        let constraints =
            typeConstraints
            |> Seq.tryPick (fun c ->
                match c.Name.ToString() with
                | x when name = x -> Some c
                | _ -> None )
        let parameter = {
            Identity = identity;
            Modifier = parameter.VarianceKeyword.Text;
            Constraints = 
                match constraints with
                | Some x -> 
                    ([], x.Constraints) 
                    ||> Seq.fold (fun acc c -> c.ToString()::acc) 
                    |> List.toSeq
                | _ -> Seq.empty }
        parameter

    let getGenericParameters (node : SyntaxNode) =
        match node with
        | GenericNode info ->
            info.Parameters
            |> Seq.map (info.Constraints |> createGenericDefinition)
        | _ -> Seq.empty

    let getAccessModifiers (node : SyntaxNode) = 
        match node with
        | TypeNode info ->
            info.Modifiers
            |> Seq.filter (fun c -> 
                accessModifiersList
                |> Set.contains (c.Kind()))
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty

    let getAdditionalModifiers (node : SyntaxNode) = 
        match node with
        | TypeNode info ->
            info.Modifiers
            |> Seq.filter (fun c -> 
                accessModifiersList 
                |> Set.contains (c.Kind()) 
                |> not)
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty

    let getBaseTypes (node : SyntaxNode) =
        match node with
        | InheritableNode info -> 
            info.BaseTypes 
            |> Seq.map (fun c -> c.Type.ToString())
        | _ -> Seq.empty