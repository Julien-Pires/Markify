namespace Markify.Services.Roslyn.VisualBasic

open DefinitionFactoryHelper
open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module DelegateDefinitionFactory =
    let create (delegateSyntax : DelegateStatementSyntax) =
        let genericParameters = getGenericParameterDefinitions delegateSyntax.TypeParameterList 
        let identity = {
            (createTypeIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (genericParameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = genericParameters }
        Delegate { 
            Identity = identity 
            Parameters = getDelegateParameters delegateSyntax.ParameterList
            ReturnType = getReturnType delegateSyntax.SubOrFunctionKeyword delegateSyntax.AsClause }

module EnumDefinitionFactory =
    let getValues (enumSyntax : EnumBlockSyntax) =
        enumSyntax.Members
        |> Seq.map (fun c -> 
            let enumValue = c :?> EnumMemberDeclarationSyntax
            {   Name = enumValue.Identifier.Text
                Value = getMemberDefaultValue enumValue.Initializer })

    let getBaseType (node : EnumBlockSyntax) =
        match node.EnumStatement.UnderlyingType with
        | null -> []
        | x -> [x.Type().ToString()]

    let create (enumSyntax : EnumBlockSyntax) =
        let identity = {
            (createTypeIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.EnumStatement.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.EnumStatement.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.EnumStatement.Modifiers 
                BaseTypes = getBaseType enumSyntax }
        Enum { 
            Identity = identity
            Values = getValues enumSyntax }

module StructureDefinitionFactory =
    let getBaseType (node : TypeBlockSyntax) =
        let types = seq {
            yield! node.Implements 
                    |> Seq.map (fun c -> c.Types)
                    |> Seq.concat
            yield! node.Inherits
                    |> Seq.map (fun c -> c.Types)
                    |> Seq.concat }
        types
        |> Seq.map (fun c -> c.ToString())
        |> Seq.toList

    let getDefaultMemberVisibility = function
        | IsInterface _ -> publicModifier
        | _ -> privateModifier

    let getMemberAccessModifiers modifiers defaultModifier =
        match getAccessModifiers modifiers with
        | [] -> defaultModifier
        | x -> x :> Modifier seq

    let getFields (structureSyntax : TypeBlockSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c -> 
            match c with
            | IsField x ->
                x.Declarators
                |> Seq.fold (fun acc2 d ->
                    let fields =
                        d.Names
                        |> Seq.map (fun e -> 
                            {   FieldDefinition.Name = e.Identifier.Text
                                Type = getTypeFromAsClause d.AsClause
                                AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                                Modifiers = getAdditionalModifiers x.Modifiers
                                DefaultValue = getMemberDefaultValue d.Initializer })
                        |> Seq.toList
                    List.append acc2 fields) acc
            | _ -> acc) []

    let containsModifiers (modifiers : SyntaxTokenList) modifier =
        modifiers
        |> Seq.exists (fun c -> c.Kind() = modifier)

    let getAccessorFromBlock (propertyBlock : PropertyBlockSyntax) accessorType defaultAccessModifier =
        let accessor =
            propertyBlock.Accessors
            |> Seq.tryFind (fun c -> c.Kind() = accessorType)
        match accessor with
        | None -> None
        | Some x ->
            let accessModifiers = getMemberAccessModifiers x.AccessorStatement.Modifiers defaultAccessModifier
            Some { AccessorDefinition.AccessModifiers = accessModifiers }

    let getAccessorFromStatement (propertyStatement : PropertyStatementSyntax) oppositeAccessorType accessModifiers =
        match containsModifiers <|| (propertyStatement.Modifiers, oppositeAccessorType) with
        | false -> Some { AccessorDefinition.AccessModifiers = accessModifiers }
        | true -> None

    let getPropertyDefinition (propertyStatement : PropertyStatementSyntax) defaultAccessModifier = {
            PropertyDefinition.Name = propertyStatement.Identifier.Text 
            AccessModifiers = getMemberAccessModifiers propertyStatement.Modifiers defaultAccessModifier
            Modifiers = getAdditionalModifiers propertyStatement.Modifiers
            Type = getTypeFromAsClause propertyStatement.AsClause
            DefaultValue = getMemberDefaultValue propertyStatement.Initializer
            IsWrite = None
            IsRead = None }

    let getPropertyFromBlock (propertyBlock : PropertyBlockSyntax) defaultAccessModifier =
        let propertyDefinition = getPropertyDefinition propertyBlock.PropertyStatement defaultAccessModifier
        let accessModifiers = propertyDefinition.AccessModifiers
        {   propertyDefinition with
                IsWrite = getAccessorFromBlock propertyBlock SyntaxKind.SetAccessorBlock accessModifiers
                IsRead = getAccessorFromBlock propertyBlock SyntaxKind.GetAccessorBlock accessModifiers }

    let getPropertyFromStatement propertyStatement defaultAccessModifier = 
        let propertyDefinition = getPropertyDefinition propertyStatement defaultAccessModifier
        let accessModifiers = propertyDefinition.AccessModifiers
        {   propertyDefinition with
                IsWrite = getAccessorFromStatement propertyStatement SyntaxKind.ReadOnlyKeyword accessModifiers
                IsRead = getAccessorFromStatement propertyStatement SyntaxKind.WriteOnlyKeyword accessModifiers }

    let getProperties (structureSyntax : TypeBlockSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c -> 
            match c with
            | IsPropertyBlock x -> (getPropertyFromBlock x defaultAccessModifier)::acc
            | IsPropertyStatement x -> (getPropertyFromStatement x defaultAccessModifier)::acc
            | _ -> acc) []

    let getEventFromStatement (event : EventStatementSyntax) defaultAccessModifier = {
        EventDefinition.Name = event.Identifier.Text 
        Type = getTypeFromAsClause event.AsClause
        AccessModifiers = getMemberAccessModifiers event.Modifiers defaultAccessModifier
        Modifiers = getAdditionalModifiers event.Modifiers }

    let getEvents (structureSyntax : TypeBlockSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c -> 
            match c with
            | IsEventStatement x -> (getEventFromStatement x defaultAccessModifier)::acc
            | IsEventBlock x -> (getEventFromStatement x.EventStatement defaultAccessModifier)::acc
            | _ -> acc) []

    let getMethods (structureSyntax : TypeBlockSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsMethod x ->
                let identity = {
                    (createTypeIdentity x) with
                        Name = SyntaxHelper.getName x.Identifier 0
                        AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = getAdditionalModifiers x.Modifiers
                        Parameters = getGenericParameterDefinitions x.TypeParameterList }
                let methodDefinition = {
                    Identity = identity
                    Parameters = getDelegateParameters x.ParameterList
                    ReturnType = getReturnType x.SubOrFunctionKeyword x.AsClause }
                methodDefinition::acc
            | _ -> acc ) []

    let createDefinition (structureSyntax : TypeBlockSyntax) =
        let parameters = getGenericParameterDefinitions structureSyntax.BlockStatement.TypeParameterList
        let modifiers = structureSyntax.BlockStatement.Modifiers
        let identity = {
            (createTypeIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.BlockStatement.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers modifiers 
                Modifiers = getAdditionalModifiers modifiers 
                BaseTypes = getBaseType structureSyntax
                Parameters = parameters }
        let defaultAccessModifier = getDefaultMemberVisibility structureSyntax
        {   Identity = identity
            Fields = getFields structureSyntax defaultAccessModifier
            Properties = getProperties structureSyntax defaultAccessModifier
            Events = getEvents structureSyntax defaultAccessModifier 
            Methods = getMethods structureSyntax defaultAccessModifier }

    let create structureSyntax =
        let definition = createDefinition structureSyntax
        match structureSyntax with
        | IsStruct _ -> Struct definition
        | IsInterface _ -> Interface definition
        | _ -> Class definition

module NamespaceDefinitionFactory =
    let create (namespaceSyntax : NamespaceBlockSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.NamespaceStatement.Name.ToString() }