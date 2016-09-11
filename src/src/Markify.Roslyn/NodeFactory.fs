namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

[<AbstractClass>]
type NodeHelper() =
    abstract member ReadSource : string -> SyntaxTree
    abstract member GetTypeName : SyntaxNode -> NodeName
    abstract member GetTypeKind : SyntaxNode -> StructureKind
    abstract member GetModifiers : SyntaxNode -> Modifier list
    abstract member GetAccessModifiers : SyntaxNode -> Modifier list
    abstract member GetParents : SyntaxNode -> BaseType list
    abstract member GetGenericConstraints : SyntaxNode -> TypeConstraint list
    abstract member GetGenericParameters : SyntaxNode -> GenericParameter list
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
        Type {
            Node = node
            Name = nodeHelper.GetTypeName node
            Kind = nodeHelper.GetTypeKind node
            Parent = parentBuilder node
            Modifiers = nodeHelper.GetModifiers node
            AccessModifiers = nodeHelper.GetAccessModifiers node
            Constraints = nodeHelper.GetGenericConstraints node
            Parameters = nodeHelper.GetGenericParameters node
            Bases = nodeHelper.GetParents node }

    let buildNamespaceNode node =
        Namespace {
            NamespaceNode.Node = node
            Name = nodeHelper.GetNamespaceName node }

    let rec buildNode node =
        let parentBuilder = buildParent buildNode
        let nodeBuilder =
            match node with
            | TypeNode _ -> buildTypeNode parentBuilder
            | NamespaceNode _ -> buildNamespaceNode
            | null -> fun c -> NoNode
            | _ -> fun c -> buildNode c.Parent
        nodeBuilder node

    member this.GetNodes source =
        let tree = nodeHelper.ReadSource source
        let root = tree.GetRoot()
        root.DescendantNodes()
        |> Seq.filter (fun c -> 
            match c with
            | TypeNode _ | NamespaceNode _ -> true
            | _ -> false )
        |> Seq.map buildNode
        |> Seq.toList