module SyntaxNodeExtension
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type TypeName = string

    [<Struct>]
    type TypeInfo =
        val Name: TypeName
        val Parameters: TypeParameterSyntax seq
        val Constraints: TypeParameterConstraintClauseSyntax seq
        val Modifiers: SyntaxToken seq
        new (name, parameters, constraints, modifiers) = 
            {Name = name; Parameters = parameters; Constraints = constraints; Modifiers = modifiers}

    let inline getTypeInfo (x : ^T) =
        let parametersList = (^T : (member TypeParameterList : TypeParameterListSyntax)(x))
        new TypeInfo(
            (^T : (member Identifier : SyntaxToken)(x)).Text,
            (
                match parametersList with
                | null -> Seq.empty<TypeParameterSyntax>
                | _ -> parametersList.Parameters :> TypeParameterSyntax seq
            ),
            (^T : (member ConstraintClauses : SyntaxList<TypeParameterConstraintClauseSyntax>)(x)),
            (^T : (member Modifiers : SyntaxTokenList)(x))
        )

    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as c -> Some(c)
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as c -> Some(c)
        | _ -> None
    
    let (|InheritableType|_|) (node: SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as c -> Some(c)
        | _ -> None

    let (|TypeNode|_|) (node: SyntaxNode) =
        match node with
        | ClassNode i -> Some(true)
        | _ -> None

    let (|TypeInfo|_|) (node: SyntaxNode) =
        match node with
        | ClassNode n -> Some(getTypeInfo n)
        | _ -> None