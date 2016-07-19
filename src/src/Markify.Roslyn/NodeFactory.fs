﻿namespace Markify.Roslyn

open Markify.Models.Definitions
open Microsoft.CodeAnalysis

[<AbstractClass>]
type NodeHelper() =
    abstract member GetTypeName : SyntaxNode -> NodeName
    abstract member GetTypeKind : SyntaxNode -> StructureKind
    abstract member GetModifiers : SyntaxNode -> Modifier seq
    abstract member GetAccessModifiers : SyntaxNode -> Modifier seq
    abstract member GetParents : SyntaxNode -> BaseType seq
    abstract member GetGenericConstraints : SyntaxNode -> TypeConstraint seq
    abstract member GetGenericParameters : SyntaxNode -> GenericParameter seq
    abstract member GetNamespaceName : SyntaxNode -> NodeName

type NodeFactory(nodeHelper : NodeHelper) =
    let nodeHelper = nodeHelper

    let getParentNode (node : SyntaxNode) getParent =
        match node.Parent with
        | null -> lazy NoNode
        | x -> lazy (getParent x)

    member this.buildTypeNode node getParent =
        let parent = getParentNode node getParent
        let typeNode = {
            Node = node
            Name = nodeHelper.GetTypeName node
            Kind = nodeHelper.GetTypeKind node
            Parent = parent
            Modifiers = nodeHelper.GetModifiers node
            AccessModifiers = nodeHelper.GetAccessModifiers node
            Constraints = nodeHelper.GetGenericConstraints node
            Parameters = nodeHelper.GetGenericParameters node
            Bases = nodeHelper.GetParents node}
        Type typeNode

    member this.buildNamespaceNode node =
        let namespaceNode = {
            NamespaceNode.Node = node
            Name = nodeHelper.GetNamespaceName node}
        Namespace namespaceNode

    member this.buildOtherNode node getParent =
        let parent = getParentNode node getParent
        let otherNode = {
            Node = node
            Parent = parent}
        Other otherNode