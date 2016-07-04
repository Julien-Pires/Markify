namespace Markify.Roslyn

open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax

module CSharpHelper = 
    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as c -> Some c
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as c -> Some c
        | _ -> None
        
    let (|InterfaceNode|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceDeclarationSyntax as c -> Some c
        | _ -> None

    let (|StructNode|_|) (node : SyntaxNode) =
        match node with
        | :? StructDeclarationSyntax as c -> Some c
        | _ -> None

    let (|EnumNode|_|) (node : SyntaxNode) =
        match node with
        | :? EnumDeclarationSyntax as c -> Some c
        | _ -> None

    let (|DelegateNode|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax as c -> Some c
        | _ -> None

    let (|ContainerTypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ClassNode c -> Some (c :> TypeDeclarationSyntax)
        | InterfaceNode c -> Some (c :> TypeDeclarationSyntax)
        | StructNode c -> Some (c :> TypeDeclarationSyntax)
        | _ -> None

    let (|TypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ClassNode x -> Some true
        | InterfaceNode x -> Some true
        | StructNode x -> Some true
        | EnumNode x -> Some true
        | DelegateNode x -> Some true
        | _ -> None

    let getTypeKind (node : SyntaxNode) =
        match node with
        | ClassNode _ -> StructureKind.Class
        | InterfaceNode _ -> StructureKind.Interface
        | StructNode _ -> StructureKind.Struct
        | EnumNode _ -> StructureKind.Enum
        | DelegateNode _ -> StructureKind.Delegate
        | _ -> StructureKind.Unknown

    let getTypeConstraint (node : SyntaxNode) =
        let constraintsList =
            match node with
            | ContainerTypeNode x -> x.ConstraintClauses :> TypeParameterConstraintClauseSyntax seq
            | DelegateNode x -> x.ConstraintClauses :> TypeParameterConstraintClauseSyntax seq
            | _ -> Seq.empty

        constraintsList
        |> Seq.map (fun c ->
            let typesConstraints = 
                c.Constraints
                |> Seq.map (fun c -> c.ToString())
            let constraints = {
                TypeConstraints.Name = c.Name.ToString()
                Constraints = typesConstraints}
            constraints)

    let addGenericsInfo (t : TypeNode) =
        let node = t.Node
        let result = { t with
                        Constraints = getTypeConstraint node}
        result

    let rec getNode node =
        match node with
        | TypeNode _ ->
            match node with
            | ContainerTypeNode x ->
                let kind = getTypeKind node
                (x, kind, getNode)
                |||> buildTypeNode addGenericsInfo 
            | EnumNode x -> 
                (x, StructureKind.Enum, getNode)
                |||> buildTypeNode (fun c -> c)
            | DelegateNode x -> 
                (x, StructureKind.Delegate, getNode)
                |||> buildTypeNode addGenericsInfo 
            | _ -> null
        | NamespaceNode x -> buildNamespaceNode x
        | null -> NoNode
        | _ -> buildOtherNode node getNode