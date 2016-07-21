namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

[<AbstractClass>]
type NodeHelper() =
    abstract member ReadSource : string -> SyntaxTree
    abstract member GetTypeName : SyntaxNode -> NodeName
    abstract member GetTypeKind : SyntaxNode -> StructureKind
    abstract member GetModifiers : SyntaxNode -> Modifier seq
    abstract member GetAccessModifiers : SyntaxNode -> Modifier seq
    abstract member GetParents : SyntaxNode -> BaseType seq
    abstract member GetGenericConstraints : SyntaxNode -> TypeConstraint seq
    abstract member GetGenericParameters : SyntaxNode -> GenericParameter seq
    abstract member GetNamespaceName : SyntaxNode -> NodeName
    abstract member IsTypeNode : SyntaxNode -> bool option
    abstract member IsNamespaceNode : SyntaxNode -> bool option

type NodeFactory(nodeHelper : NodeHelper) =
    let nodeHelper = nodeHelper

    let (|TypeNode|_|) node =
        nodeHelper.IsTypeNode node

    let (|NamespaceNode|_|) node =
        nodeHelper.IsNamespaceNode node

    let buildParent nodeBuilder (node : SyntaxNode) =
        match node.Parent with
        | null -> lazy NoNode
        | x -> lazy (nodeBuilder x)

    let buildTypeNode parentBuilder node =
        let typeNode = {
            Node = node
            Name = nodeHelper.GetTypeName node
            Kind = nodeHelper.GetTypeKind node
            Parent = parentBuilder node
            Modifiers = nodeHelper.GetModifiers node
            AccessModifiers = nodeHelper.GetAccessModifiers node
            Constraints = nodeHelper.GetGenericConstraints node
            Parameters = nodeHelper.GetGenericParameters node
            Bases = nodeHelper.GetParents node}
        Type typeNode

    let buildNamespaceNode node =
        let namespaceNode = {
            NamespaceNode.Node = node
            Name = nodeHelper.GetNamespaceName node}
        Namespace namespaceNode

    let buildOtherNode parentBuilder node =
        let otherNode = {
            Node = node
            Parent = parentBuilder node}
        Other otherNode

    let rec buildNode node =
        let parentBuilder = buildParent buildNode
        let nodeBuilder =
            match node with
            | TypeNode _ -> Some (buildTypeNode parentBuilder)
            | NamespaceNode _ -> Some (buildNamespaceNode)
            | null -> None
            | _ -> Some (buildOtherNode parentBuilder)
        let newNode =
            match nodeBuilder with
            | Some x -> x node
            | None -> NoNode
        newNode

    member this.GetNodes source =
        let tree = nodeHelper.ReadSource source
        let root = tree.GetRoot()
        root.DescendantNodes()
        |> Seq.filter (fun c -> 
            match c with
            | TypeNode _ -> true
            | _ -> false)
        |> Seq.map buildNode