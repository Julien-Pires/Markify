namespace Markify.Services.Roslyn.Csharp

open System
open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module DefinitionFactoryHelper =
    let getStructureTypeIdentifier = function
        | IsStructureType x -> Some x.Identifier
        | _ -> None

    let getNamespaceIdentifier = function
        | IsNamespace x -> Some (x.Name :> SyntaxNode)
        | _ -> None

    let createIdentity : SyntaxNode -> TypeIdentity =
        DefinitionHelper.createIdentity getStructureTypeIdentifier getNamespaceIdentifier

    let getGenericParameters = function
        | null -> Seq.empty
        | (w : TypeParameterListSyntax) -> w.Parameters :> TypeParameterSyntax seq

    let getGenericConstraints (constraints : TypeParameterConstraintClauseSyntax seq) =
        constraints
        |> Seq.map (fun c ->
            let typesConstraints =
                c.Constraints
                |> Seq.map (fun d -> d.ToString())
                |> Seq.toList
            (c.Name.ToString(), typesConstraints))
        |> Map.ofSeq

    let findConstraints constraints name =
        constraints
        |> Map.tryFind name
        |> function 
            | Some x -> x 
            | None -> []

    let getGenericParameterDefinitions parameters constraints =
        let constraintsMap = getGenericConstraints constraints
        getGenericParameters parameters
        |> Seq.map (fun c ->
            let name = c.Identifier.Text
            let variance = 
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some <| x.ToString()
            let constraints = findConstraints constraintsMap name
            {   Name = name
                Modifier = variance
                Constraints = constraints })
        |> Seq.toList

    let getBaseTypes (node : BaseTypeDeclarationSyntax) =
        match node.BaseList with
        | null -> []
        | x ->
            x.Types
            |> Seq.map (fun c -> c.Type.ToString())
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

    let getDefaultMemberVisibility = function
        | IsInterface _ | IsEnum _ -> publicModifier
        | _ -> privateModifier

    let getMemberDefaultValue (initializer : EqualsValueClauseSyntax) =
        match initializer with
        | null -> None
        | x -> Some <| x.Value.ToString()

    let getDelegateParameters (parametersList : ParameterListSyntax) =
        parametersList.Parameters
        |> Seq.map (fun c ->
            let parameterType =
                 match c.Type with
                 | null -> c.Identifier.Text
                 | _ -> c.Type.ToString()
            {   ParameterDefinition.Name = c.Identifier.Text 
                Type = parameterType
                Modifier = (getAdditionalModifiers c.Modifiers) |> List.tryHead 
                DefaultValue = getMemberDefaultValue c.Default })
        |> Seq.toList

    let getTypeComments node = {
        Comments = CommentBuilder.getComments node }