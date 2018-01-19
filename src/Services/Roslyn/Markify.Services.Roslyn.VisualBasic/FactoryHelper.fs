namespace Markify.Services.Roslyn.VisualBasic

open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open Markify.Services.Roslyn.VisualBasic
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module DefinitionFactoryHelper =
    let getStructureIdentifier = function
        | IsStructureType x -> Some x.BlockStatement.Identifier
        | _ -> None

    let getNamespaceIdentifier = function
        | IsNamespace x -> Some (x.NamespaceStatement.Name :> SyntaxNode)
        | _ -> None

    let createIdentity : SyntaxNode -> TypeIdentity =
        DefinitionHelper.createIdentity getStructureIdentifier getNamespaceIdentifier

    let getGenericParameters = function
        | null -> Seq.empty
        | (w : TypeParameterListSyntax) -> w.Parameters :> TypeParameterSyntax seq

    let getGenericConstraints (parameter : TypeParameterSyntax) =
        let constraints =
            match parameter.TypeParameterConstraintClause with
            | :? TypeParameterSingleConstraintClauseSyntax as x -> SeparatedSyntaxList().Add x.Constraint
            | :? TypeParameterMultipleConstraintClauseSyntax as x -> x.Constraints
            | null | _ -> SeparatedSyntaxList()
        constraints
        |> Seq.map (fun c -> c.ToString()) 
        |> Seq.toList

    let getGenericParameterDefinitions parameters =
        getGenericParameters parameters
        |> Seq.map (fun c ->
            let constraints = getGenericConstraints c
            let modifier =
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some <| x.ToString()
            {   Name = c.Identifier.Text
                Modifier = modifier
                Constraints = constraints })
        |> Seq.toList

    let filterModifiers filter (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter filter
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let getAccessModifiers modifiers =
        modifiers
        |> filterModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind()))

    let getAdditionalModifiers modifiers =
        modifiers
        |> filterModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind())
            |> not)

    let getTypeFromAsClause (clause : AsClauseSyntax) =
        match clause with
        | null -> "Object"
        | _ -> clause.Type().ToString()

    let getReturnType (delegateType : SyntaxToken) clause =
        match delegateType.Kind() with
        | SyntaxKind.SubKeyword -> "Void"
        | _ -> getTypeFromAsClause clause

    let getDefaultMemberVisibility = function
        | IsInterface _ -> publicModifier
        | _ -> privateModifier

    let getMemberDefaultValue (initializer : EqualsValueSyntax) =
        match initializer with
        | null -> None
        | x -> Some <| x.Value.ToString()

    let getDelegateParameters (parametersList : ParameterListSyntax) =
        parametersList.Parameters
        |> Seq.map (fun c -> 
            {   ParameterDefinition.Name = c.Identifier.Identifier.Text 
                Type = getTypeFromAsClause c.AsClause
                Modifier = (getAdditionalModifiers c.Modifiers) |> List.tryHead 
                DefaultValue = getMemberDefaultValue c.Default })
        |> Seq.toList

    let getTypeComments node = {
        Comments = CommentBuilder.getComments node }