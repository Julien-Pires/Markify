namespace Markify.Services.Roslyn.VisualBasic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic.Syntax

[<AutoOpen>]
module VisualBasicSyntaxHelper =
    let (|IsNamespace|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsClass|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassBlockSyntax as x -> Some x
        | _ -> None

    let (|IsInterface|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsStruct|_|) (node : SyntaxNode) =
        match node with
        | :? StructureBlockSyntax as x -> Some x
        | _ -> None

    let (|IsEnum|_|) (node : SyntaxNode) =
        match node with
        | :? EnumBlockSyntax as x -> Some x
        | _ -> None

    let (|IsDelegate|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateStatementSyntax as x -> Some x
        | _ -> None

    let (|IsStructureType|_|) (node : SyntaxNode) =
        match node with
        | IsClass x -> Some (x :> TypeBlockSyntax)
        | IsStruct x -> Some (x :> TypeBlockSyntax)
        | IsInterface x -> Some (x :> TypeBlockSyntax)
        | _ -> None

    let (|IsPropertyStatement|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyStatementSyntax as x -> Some x
        | _ -> None

    let (|IsPropertyBlock|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyBlockSyntax as x -> Some x
        | _ -> None

    let (|IsProperty|_|) (node : SyntaxNode) =
        match node with
        | IsPropertyBlock x -> Some (x :> DeclarationStatementSyntax)
        | IsPropertyStatement x -> Some (x :> DeclarationStatementSyntax)
        | _ -> None

    let (|IsField|_|) (node : SyntaxNode) =
        match node with
        | :? FieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsMethod|_|) (node : SyntaxNode) =
        match node with
        | :? MethodStatementSyntax as x -> Some x
        | :? MethodBlockSyntax as x -> Some (x.BlockStatement :?> MethodStatementSyntax)
        | _ -> None

    let (|IsEvent|_|) (node : SyntaxNode) =
        match node with
        | :? EventBlockSyntax as x -> Some x.EventStatement
        | :? EventStatementSyntax as x -> Some x
        | _ -> None

open Microsoft.CodeAnalysis.VisualBasic

[<AutoOpen>]
module VisualBasicKeywordHelper =
    let publicModifier = [SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text]
    let privateModifier = [SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text]
    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.FriendKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]