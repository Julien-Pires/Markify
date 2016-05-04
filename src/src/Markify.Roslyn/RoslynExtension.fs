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

    let inline adaptStructure (x : ^T) =
        let parametersList = (^T : (member TypeParameterList : TypeParameterListSyntax)(x))
        new StructureInfo(
            (^T : (member Identifier : SyntaxToken)(x)).Text,
            (
                match parametersList with
                | null -> Seq.empty<TypeParameterSyntax>
                | _ -> parametersList.Parameters :> TypeParameterSyntax seq
            ),
            (^T : (member ConstraintClauses : SyntaxList<TypeParameterConstraintClauseSyntax>)(x)),
            (^T : (member Modifiers : SyntaxTokenList)(x))
        )

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? TypeDeclarationSyntax as c -> Some(adaptStructure c)
        | _ -> None

    let (|DelegateNode|_|) (node: SyntaxNode) = None

    let (|StructureNode|_|) (node: SyntaxNode) =
        match node with
        | ClassNode i -> Some(i)
        | DelegateNode -> None
        | _ -> None

    let getFullname (node : SyntaxNode) : Fullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc: list<string> =
            match innerNode with
            | StructureNode structure -> 
                let parametersLength = Seq.length structure.Parameters
                let name =
                    match parametersLength with
                    | 0 -> structure.Name
                    | _ -> sprintf "%s`%i" structure.Name parametersLength

                loopParentNode innerNode.Parent (name::acc)
            | :? NamespaceDeclarationSyntax as namespaceNode -> namespaceNode.Name.ToString()::acc
            | null -> acc
            | _ -> loopParentNode innerNode.Parent acc

        loopParentNode node []