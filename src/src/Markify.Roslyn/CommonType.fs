namespace Markify.Roslyn

open Microsoft.CodeAnalysis
open Markify.Models.Definitions

type ConstraintName = string
type ConstraintTypeName = string
type TypeConstraint = {
    Name : ConstraintName
    Constraints : ConstraintTypeName list
}

type ParameterName = string
type GenericVariance = string
type GenericParameter = {
    Name : ParameterName
    Modifier : GenericVariance
}

type NodeName = string
type Modifier = string
type BaseType = NodeName
type TypeNode = {
    Node : SyntaxNode
    Name : NodeName
    Kind : StructureKind
    Parent : Node Lazy
    Modifiers : Modifier list
    AccessModifiers : Modifier list
    Constraints : TypeConstraint list
    Parameters : GenericParameter list
    Bases : BaseType list
}
and NamespaceNode = {
    Node : SyntaxNode
    Name : NodeName
}
and Node = 
    | Type of TypeNode
    | Namespace of NamespaceNode
    | NoNode