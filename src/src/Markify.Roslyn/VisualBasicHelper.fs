namespace Markify.Roslyn

open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module VisualBasicHelper =
    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceBlockSyntax as c -> Some c
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassBlockSyntax as c -> Some c
        | _ -> None

    let (|InterfaceNode|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceBlockSyntax as c -> Some c
        | _ -> None

    let (|StructNode|_|) (node : SyntaxNode) =
        match node with
        | :? StructureBlockSyntax as c -> Some c
        | _ -> None

    let (|EnumNode|_|) (node : SyntaxNode) =
        match node with
        | :? EnumBlockSyntax as c -> Some c
        | _ -> None

    let (|DelegateNode|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateStatementSyntax as c -> Some c
        | _ -> None

    let (|TypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ClassNode x -> Some true
        | InterfaceNode x -> Some true
        | StructNode x -> Some true
        | EnumNode x -> Some true
        | DelegateNode x -> Some true
        | _ -> None

    let (|ContainerTypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ClassNode c -> Some (c.ClassStatement :> TypeStatementSyntax)
        | InterfaceNode c -> Some (c.InterfaceStatement :> TypeStatementSyntax)
        | StructNode c -> Some (c.StructureStatement :> TypeStatementSyntax)
        | _ -> None

    let getTypeKind (node : SyntaxNode) =
        match node with
        | ClassNode _ -> StructureKind.Class
        | InterfaceNode _ -> StructureKind.Interface
        | StructNode _ -> StructureKind.Struct
        | EnumNode _ -> StructureKind.Enum
        | DelegateNode _ -> StructureKind.Delegate
        | _ -> StructureKind.Unknown

    let rec getNode node =
        let a = fun c -> c
        match node with
        | TypeNode _ ->
            let a = fun c -> c
            match node with
            | ContainerTypeNode x -> 
                let kind = getTypeKind node
                buildTypeNode a x kind getNode
            | EnumNode x -> buildTypeNode a x.EnumStatement StructureKind.Enum getNode
            | DelegateNode x -> buildTypeNode a x StructureKind.Delegate getNode
            | _ -> null
        | NamespaceNode x -> buildNamespaceNode x.NamespaceStatement
        | null -> NoNode
        | _ -> buildOtherNode node getNode

        