module RoslynExtension
    open Representation
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type StructureName = string

    [<Struct>]
    type StructureInfo =
        val Name: StructureName
        val Parameters: TypeParameterSyntax seq
        val Constraints: TypeParameterConstraintClauseSyntax seq
        val Modifiers: SyntaxToken seq
        new (name, parameters, constraints, modifiers) = 
            {Name = name; Parameters = parameters; Constraints = constraints; Modifiers = modifiers}

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as c ->
            Some(
                new StructureInfo(
                    c.Identifier.Text,
                    (
                        match c.TypeParameterList with
                        | null -> Seq.empty<TypeParameterSyntax>
                        | _ -> c.TypeParameterList.Parameters :> TypeParameterSyntax seq
                    ),
                    c.ConstraintClauses :> TypeParameterConstraintClauseSyntax seq,
                    c.Modifiers :> SyntaxToken seq)
            )
        | _ -> None

    let (|DelegateNode|_|) (node: SyntaxNode) = None

    let (|StructureNode|_|) (node: SyntaxNode) =
        match node with
        | ClassNode i -> None
        | DelegateNode -> None
        | _ -> None

    let getFullname (node : SyntaxNode) : Fullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc: list<string> =
            match innerNode with
            | :? BaseTypeDeclarationSyntax as typeNode -> loopParentNode innerNode.Parent (typeNode.Identifier.Text::acc)
            | :? DelegateDeclarationSyntax as delegateNode -> loopParentNode innerNode.Parent (delegateNode.Identifier.Text::acc)
            | :? NamespaceDeclarationSyntax as namespaceNode -> namespaceNode.Name.ToString()::acc
            | _ -> acc

        loopParentNode node []