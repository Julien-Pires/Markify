namespace Markify.Roslyn

open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
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

    let (|GenericTypeNode|_|) (node : SyntaxNode) =
        match node with
        | :? TypeDeclarationSyntax as c -> Some c
        | _ -> None

    let (|StructureNode|_|) (node : SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as c -> Some c
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
        match node with
        | GenericTypeNode c ->
            let kind =  getTypeKind node
            getGenericTypeNode c kind getNode
        | DelegateNode c ->
            let kind =  getTypeKind node
            getGenericTypeNode c kind getNode
        | EnumNode c ->
            let kind =  getTypeKind node
            getTypeNode c kind getNode
        | NamespaceNode c -> getNamespaceNode c
        | null -> NoNode
        | _ -> getOtherNode node getNode

type CSharpInspector() =
    inherit LanguageInspector()

    override this.getNode node =
        CSharpHelper.getNode node