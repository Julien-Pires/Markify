namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module TypeFactory =
    let findConstraints parameter (constraints : SyntaxList<TypeParameterConstraintClauseSyntax>) =
        constraints
        |> Seq.tryFind (fun c -> c.Name.ToString() = parameter)
        |> function | Some x -> x.Constraints |> Seq.map string | None -> Seq.empty

    let extractAccessModifiers (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter (fun c -> 
            CSharpKeyword.accessModifiers 
            |> Set.contains (c.Kind()))
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let extractModifiers (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter (fun c -> 
            CSharpKeyword.accessModifiers 
            |> Set.contains (c.Kind()) 
            |> not)
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let extractGenerics (parameters : TypeParameterListSyntax) (constraints : SyntaxList<TypeParameterConstraintClauseSyntax>) =
        parameters.Parameters
        |> Seq.map (fun c ->
            let name = c.Identifier.Text
            let variance =
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some (x.ToString())
            {   Name = c.Identifier.Text
                Modifier = variance
                Constraints = constraints |> findConstraints name |> Seq.toList })
        |> Seq.toList

    let extractBaseTypes baseTypes =
        baseTypes
        |> Seq.map string 
        |> Seq.toList

    let createIdentity node =
        let name = SyntaxHelper.getName node
        let genericParameters = TypeSyntaxHelper.getGenericParameters node
        let genericConstraints = TypeSyntaxHelper.getGenericConstraints node
        match name with
        | Some x -> Success {   
            Name = x.ValueText
            AccessModifiers = TypeSyntaxHelper.getModifiers node |> extractAccessModifiers
            Modifiers = TypeSyntaxHelper.getModifiers node |> extractModifiers
            Generics = (genericParameters, genericConstraints) ||> extractGenerics
            BaseType = TypeSyntaxHelper.getBaseTypes node |> extractBaseTypes }
        | None -> Failure ""