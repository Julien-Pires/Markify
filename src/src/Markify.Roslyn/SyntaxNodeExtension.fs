namespace Markify.Roslyn

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

type NodeName = string
type TypeNode = {
    Node : SyntaxNode
    Name : NodeName
    Kind : StructureKind
    Parent : Node Lazy
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
            NamespaceNode.Name = getNamespaceName node}
        Namespace namespaceNode

    let inline getTypeNode(node : ^T) kind getParent =
        let name = getTypeName node
        let parent = getParentNode node getParent
        let typeNode = {
            Node = node
            Name = name
            Kind = kind
            Parent = parent}
        Type typeNode