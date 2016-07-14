namespace Markify.Roslyn

open Microsoft.CodeAnalysis
open Markify.Models.Definitions

type ConstraintName = string
type ConstraintTypeName = string
type TypeConstraints = {
    Name : ConstraintName
    Constraints : ConstraintTypeName seq
}

type ParameterName = string
type GenericVariance = string
type GenericParameters = {
    Name : ParameterName
    Modifier : GenericVariance
}

type NodeName = string
type Modifiers = string seq
type Parents = NodeName seq
type TypeNode = {
    Node : SyntaxNode
    Name : NodeName
    Kind : StructureKind
    Parent : Node Lazy
    Modifiers : Modifiers
    AccessModifiers : Modifiers
    Constraints : TypeConstraints seq
    Parameters : GenericParameters seq
    Bases : Parents
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