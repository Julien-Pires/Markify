namespace Markify.Roslyn

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

type TypeName = string

type NodeKind = 
    | Type of StructureKind
    | Namespace
    | Other

type TypeNode = {
    Node : SyntaxNode
    Name : TypeName
    Kind : NodeKind
    Parent : (TypeNode option) Lazy
}

module SyntaxNodeExtension =
    let inline getTypeName (x : ^T) =
        (^T : (member Identifier : SyntaxToken)(x)).Text

    let getTypeNode(node : SyntaxNode) kind parent name =
        let p =
            match node.Parent with
            | null -> lazy None
            | c -> lazy (parent c)
        let typeNode = { 
            Node = node
            Name = name node
            Kind = kind
            Parent = p}
        Some typeNode