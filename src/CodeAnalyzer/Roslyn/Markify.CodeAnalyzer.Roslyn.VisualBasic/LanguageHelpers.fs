namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open Microsoft.CodeAnalysis.VisualBasic

[<AutoOpen>]
module VisualBasicKeyword =
    let publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text

    let privateModifier = SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text

    let accessModifiers = Set [
        SyntaxKind.PublicKeyword
        SyntaxKind.FriendKeyword 
        SyntaxKind.PrivateKeyword
        SyntaxKind.ProtectedKeyword ]