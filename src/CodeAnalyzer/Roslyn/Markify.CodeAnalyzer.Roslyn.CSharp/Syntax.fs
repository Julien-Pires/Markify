namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

[<AutoOpen>]
module CSharpSyntaxHelper =
    let (|IsNamespace|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsClass|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as x -> Some x
        | _ -> None
        
    let (|IsInterface|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsStruct|_|) (node : SyntaxNode) =
        match node with
        | :? StructDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEnum|_|) (node : SyntaxNode) =
        match node with
        | :? EnumDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsDelegate|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsStructureType|_|) (node : SyntaxNode) = 
        match node with
        | IsClass x -> Some (x :> TypeDeclarationSyntax)
        | IsStruct x -> Some (x :> TypeDeclarationSyntax)
        | IsInterface x -> Some (x :> TypeDeclarationSyntax)
        | _ -> None

    let (|IsProperty|_|) (node : SyntaxNode) = 
        match node with
        | :? PropertyDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsField|_|) (node : SyntaxNode) =
        match node with
        | :? FieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsMethod|_|) (node : SyntaxNode) =
        match node with
        | :? MethodDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEventDeclaration|_|) (node : SyntaxNode) =
        match node with
        | :? EventDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEventField|_|) (node : SyntaxNode) = 
        match node with
        | :? EventFieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEvent|_|) (node : SyntaxNode) = 
        match node with
        | IsEventDeclaration x -> Some (x :> MemberDeclarationSyntax)
        | IsEventField x -> Some (x :> MemberDeclarationSyntax)
        | _ -> None

[<AutoOpen>]
module CSharpKeywordHelper =
    let publicModifier = [SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text]
    let privateModifier = [SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text]
    let accessModifiersList =
        Set [   SyntaxKind.PublicKeyword
                SyntaxKind.InternalKeyword 
                SyntaxKind.PrivateKeyword
                SyntaxKind.ProtectedKeyword ]