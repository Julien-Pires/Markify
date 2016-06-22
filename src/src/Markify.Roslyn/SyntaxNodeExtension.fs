module SyntaxNodeExtension
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp.Syntax

    type TypeName = string

    type TypeInfo = {
        Name : TypeName
        Modifiers : SyntaxToken seq
    }

    type GenericInfo = {
        Parameters : TypeParameterSyntax seq
        Constraints : TypeParameterConstraintClauseSyntax SyntaxList
    }

    type InheritanceInfo = {
        BaseTypes : BaseTypeSyntax seq
    }

    let inline getTypeInfo (x : ^T) = {
        Name = (^T : (member Identifier : SyntaxToken)(x)).Text;
        Modifiers = (^T : (member Modifiers : SyntaxTokenList)(x)) }

    let inline getGenericInfo (x : ^T) =
        let parameters =
            match (^T : (member TypeParameterList : TypeParameterListSyntax)(x)) with
            | null -> Seq.empty
            | c -> c.Parameters :> TypeParameterSyntax seq
        let info = {
            Parameters = parameters;
            Constraints = (^T : (member ConstraintClauses : TypeParameterConstraintClauseSyntax SyntaxList)(x)) }
        info

    let inline getInheritanceInfo (x : ^T) = {
        BaseTypes =
            match (^T : (member BaseList : BaseListSyntax)(x)) with
            | null -> Seq.empty
            | c -> c.Types :> BaseTypeSyntax seq }

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

    let (|InheritableNode|_|) (node: SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as c -> getInheritanceInfo c |> Some
        | _ -> None

    let (|GenericNode|_|) (node : SyntaxNode) =
        match node with
        | :? TypeDeclarationSyntax as c -> getGenericInfo c |> Some
        | :? DelegateDeclarationSyntax as c -> getGenericInfo c |> Some
        | _ -> None

    let (|TypeNode|_|) (node : SyntaxNode) = 
        match node with
        | :? BaseTypeDeclarationSyntax as c -> getTypeInfo c |> Some
        | :? DelegateDeclarationSyntax as c -> getTypeInfo c |> Some
        | _ -> None