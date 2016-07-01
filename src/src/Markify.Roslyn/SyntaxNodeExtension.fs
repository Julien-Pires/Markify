namespace Markify.Roslyn

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

type ConstraintName = string
type ConstraintTypeName = string
type TypeConstraints = {
    Name : ConstraintName
    Constraints : ConstraintTypeName seq
}

type ParameterName = string
type GenericParameters = {
    Name : ParameterName
    Modifier : VarianceKind
}

type NodeName = string
type TypeNode = {
    Node : SyntaxNode
    Name : NodeName
    Kind : StructureKind
    Parent : Node Lazy
    Constraints : TypeConstraints seq
    Parameters : GenericParameters seq
}
and NamespaceNode = {
    Node : SyntaxNode
    Name : NodeName
}
and OtherNode = {
    Node : SyntaxNode
    Parent : Node Lazy
}
and Node = 
    | Type of TypeNode
    | Namespace of NamespaceNode
    | Other of OtherNode
    | NoNode

module SyntaxNodeExtension =
    let inline getTypeName (x : ^T) =
        (^T : (member Identifier : SyntaxToken)(x)).Text

    let inline getNamespaceName (x : ^T) =
        (^T : (member Name : ^a)(x)).ToString()

    let inline getTypeConstraint (x : ^T) =
        (^T : (member ConstraintClauses : ^a SyntaxList)(x))
        |> Seq.map (fun c ->
            let typesConstraints = 
                (^a : (member Constraints : ^b SeparatedSyntaxList)(c)) 
                |> Seq.map (fun c -> c.ToString())
            let constraints = {
                TypeConstraints.Name = (^a : (member Name : ^c)(c)).ToString()
                Constraints = typesConstraints}
            constraints)

    let inline getGenericParameters (x : ^T) =
        let parametersList = (^T : (member TypeParameterList : ^a)(x))
        match parametersList with
        | null -> Seq.empty
        | _ ->
            (^a : (member Parameters : 'b SeparatedSyntaxList)(parametersList))
            |> Seq.map (fun c -> 
                let name = (^b : (member Identifier : SyntaxToken)(c)).Text
                let modifier = (^b : (member VarianceKeyword : SyntaxToken)(c)).Value :?> VarianceKind
                let param = {
                    Name = name
                    Modifier = modifier}
                param)


    let getParentNode (node : SyntaxNode) getParent =
        match node.Parent with
        | null -> lazy NoNode
        | c -> lazy (getParent c)

    let getOtherNode node getParent =
        let parent = getParentNode node getParent
        let otherNode = {
            Node = node
            Parent = parent}
        Other otherNode

    let inline getNamespaceNode (node : ^T) =
        let namespaceNode = {
            NamespaceNode.Node = node
            Name = getNamespaceName node}
        Namespace namespaceNode

    let inline constructTypeNode(node : ^T) kind getParent =
        let name = getTypeName node
        let parent = getParentNode node getParent
        let typeNode = {
            Node = node
            Name = name
            Kind = kind
            Parent = parent
            Constraints = Seq.empty
            Parameters = Seq.empty}
        typeNode

    let inline getTypeNode(node : ^T) kind getParent =
        (node, kind, getParent)
        |||> constructTypeNode
        |> Type

    let inline getGenericTypeNode(node : ^T) kind getParent =
        (node, kind, getParent)
        |||> constructTypeNode
        |> fun c -> { c with
                        Constraints = getTypeConstraint node 
                        Parameters = getGenericParameters node}
        |> Type