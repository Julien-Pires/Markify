namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

[<AutoOpen>]
module VisualBasicSyntaxHelper =
    let (|IsNamespace|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsClass|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassBlockSyntax as x -> Some x
        | _ -> None

    let (|IsInterface|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsStruct|_|) (node : SyntaxNode) =
        match node with
        | :? StructureBlockSyntax as x -> Some x
        | _ -> None

    let (|IsEnum|_|) (node : SyntaxNode) =
        match node with
        | :? EnumBlockSyntax as x -> Some x
        | _ -> None

    let (|IsDelegate|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateStatementSyntax as x -> Some x
        | _ -> None

    let (|IsPropertyStatement|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyStatementSyntax as x -> Some x
        | _ -> None

    let (|IsPropertyBlock|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyBlockSyntax as x -> Some x
        | _ -> None

    let (|IsField|_|) (node : SyntaxNode) =
        match node with
        | :? FieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsMethod|_|) (node : SyntaxNode) =
        match node with
        | :? MethodStatementSyntax as x -> Some x
        | :? MethodBlockSyntax as x -> Some (x.BlockStatement :?> MethodStatementSyntax)
        | _ -> None

    let (|IsEvent|_|) (node : SyntaxNode) =
        match node with
        | :? EventBlockSyntax as x -> Some x.EventStatement
        | :? EventStatementSyntax as x -> Some x
        | _ -> None

module SyntaxHelper =
    let getName (node : SyntaxNode)  =
        match node with
        | :? TypeBlockSyntax as x -> Some x.BlockStatement.Identifier
        | :? EnumBlockSyntax as x -> Some x.EnumStatement.Identifier
        | :? DelegateStatementSyntax as x -> Some x.Identifier
        | _ -> None

module TypeSyntaxHelper =
    let getModifiers (node : SyntaxNode) =
        match node with
        | :? TypeBlockSyntax as x -> x.BlockStatement.Modifiers
        | :? EnumBlockSyntax as x -> x.EnumStatement.Modifiers
        | :? DelegateStatementSyntax as x -> x.Modifiers
        | _ -> SyntaxTokenList()
    
    let getBaseTypes (node : SyntaxNode) =
        match node with
        | :? TypeBlockSyntax as x -> seq {
            yield! x.Inherits |> Seq.collect (fun c -> c.Types)
            yield! x.Implements |> Seq.collect (fun c -> c.Types) } |> Seq.toList
        | :? EnumBlockSyntax as x ->
            match x.EnumStatement.UnderlyingType with
            | null -> []
            | x -> [x.Type()]
        | _ -> []

    let getGenericParameters (node : SyntaxNode) =
        let parameters = 
            match node with
            | :? TypeBlockSyntax as x -> x.BlockStatement.TypeParameterList
            | :? DelegateStatementSyntax as x -> x.TypeParameterList
            | :? MethodStatementSyntax as x -> x.TypeParameterList
            | _ -> SyntaxFactory.TypeParameterList()
        match parameters with
        | null -> SyntaxFactory.TypeParameterList()
        | x -> x

    let getGenericConstraints (parameter : TypeParameterSyntax) =
        match parameter.TypeParameterConstraintClause with
        | :? TypeParameterSingleConstraintClauseSyntax as x -> SeparatedSyntaxList().Add x.Constraint
        | :? TypeParameterMultipleConstraintClauseSyntax as x -> x.Constraints
        | null | _ -> SeparatedSyntaxList()