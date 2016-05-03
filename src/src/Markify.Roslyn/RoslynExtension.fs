module RoslynExtension
    open Representation
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp.Syntax

    let getFullname (node : SyntaxNode) : Fullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc: list<string> =
            match innerNode with
            | :? BaseTypeDeclarationSyntax as typeNode -> loopParentNode innerNode.Parent (typeNode.Identifier.Text::acc)
            | :? DelegateDeclarationSyntax as delegateNode -> loopParentNode innerNode.Parent (delegateNode.Identifier.Text::acc)
            | :? NamespaceDeclarationSyntax as namespaceNode -> namespaceNode.Name.ToString()::acc
            | _ -> acc

        loopParentNode node []