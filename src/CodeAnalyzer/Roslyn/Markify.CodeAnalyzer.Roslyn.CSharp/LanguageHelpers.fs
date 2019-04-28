namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Microsoft.CodeAnalysis.CSharp

[<AutoOpen>]
module CSharpKeyword =
    let publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text

    let privateModifier = SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text

    let accessModifiers = Set [
        SyntaxKind.PublicKeyword
        SyntaxKind.InternalKeyword 
        SyntaxKind.PrivateKeyword
        SyntaxKind.ProtectedKeyword ]