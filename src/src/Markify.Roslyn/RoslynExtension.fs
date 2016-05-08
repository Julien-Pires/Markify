module RoslynExtension
    open Representation
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open Microsoft.FSharp.Collections

    type StructureName = string

    [<Struct>]
    type StructureInfo =
        val Name: StructureName
        val Parameters: TypeParameterSyntax seq
        val Constraints: TypeParameterConstraintClauseSyntax seq
        val Modifiers: SyntaxToken seq
        new (name, parameters, constraints, modifiers) = 
            {Name = name; Parameters = parameters; Constraints = constraints; Modifiers = modifiers}
    
    let accessModifiersList = Set [ 
                                SyntaxKind.PublicKeyword
                                SyntaxKind.InternalKeyword 
                                SyntaxKind.PrivateKeyword
                                SyntaxKind.ProtectedKeyword ]

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
    
    let name (node : SyntaxNode) =
        match node with
        | StructureNode structure ->
            let parametersLength = Seq.length structure.Parameters
            match parametersLength with
            | 0 -> Some(structure.Name)
            | _ -> Some(sprintf "%s`%i" structure.Name parametersLength)
        | :? NamespaceDeclarationSyntax as namespaceNode -> Some(namespaceNode.Name.ToString())
        | _ -> None

    let fullname (node : SyntaxNode) : Fullname =
        let rec loopParentNode (innerNode: SyntaxNode) acc: seq<string> =
            match innerNode with
            | StructureNode s -> 
                acc
                |> Seq.append (Seq.singleton (name innerNode).Value)
                |> loopParentNode innerNode.Parent
            | :? NamespaceDeclarationSyntax as namespaceNode -> 
                Seq.append (Seq.singleton (namespaceNode.Name.ToString())) acc
            | null -> acc
            | _ -> loopParentNode innerNode.Parent acc

        loopParentNode node Seq.empty<string>

    let accessModifiers (node : SyntaxNode) = 
        match node with
        | StructureNode s ->
            s.Modifiers
            |> Seq.filter (fun c -> Set.contains (c.Kind()) accessModifiersList)
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty<string>

    let additionalModifiers (node : SyntaxNode) = 
        match node with
        | StructureNode s ->
            s.Modifiers
            |> Seq.filter (fun c -> not (Set.contains (c.Kind()) accessModifiersList))
            |> Seq.map (fun c -> c.ToString())
        | _ -> Seq.empty<string>

    let baseTypes (node : SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as baseType ->
            match baseType.BaseList with
            | null -> Seq.empty<string>
            | x -> x.Types |> Seq.map (fun c -> c.Type.ToString())
        | _ -> Seq.empty<string>