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

    let (|StructureType|_|) (node : SyntaxNode) = 
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

type CSharpInspector() =
    inherit LanguageInspector()

    let getName node : SyntaxNode -> TypeName =
        match node with
        | CSharpHelper.StructureType c -> (fun c -> (c :?> BaseTypeDeclarationSyntax).Identifier.Text)
        | _ -> (fun c -> "Unknow")

    override this.getNode node =
        match node with
        | CSharpHelper.StructureType c ->
            let kind = Type (CSharpHelper.getTypeKind node)
            let name = getName node
            (kind, this.getNode, name)
            |||> getTypeNode node
        | _ -> None