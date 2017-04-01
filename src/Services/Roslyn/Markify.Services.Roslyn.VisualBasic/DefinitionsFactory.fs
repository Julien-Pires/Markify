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
                    |> Seq.collect (fun c -> c.Types)
            yield! node.Inherits
                    |> Seq.collect (fun c -> c.Types) }
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

    let getFields (fieldSyntax : FieldDeclarationSyntax) defaultAccessModifier =
        fieldSyntax.Declarators
        |> Seq.fold (fun acc c ->
            c.Names
            |> Seq.fold (fun acc2 d ->
                let field = {
                    FieldDefinition.Name = d.Identifier.Text
                    Type = getTypeFromAsClause c.AsClause
                    AccessModifiers = getMemberAccessModifiers fieldSyntax.Modifiers defaultAccessModifier
                    Modifiers = getAdditionalModifiers fieldSyntax.Modifiers
                    DefaultValue = getMemberDefaultValue c.Initializer }
                field::acc2) acc) []

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

    let getEvent (eventSyntax : EventStatementSyntax) defaultAccessModifier = {
        EventDefinition.Name = eventSyntax.Identifier.Text 
        Type = getTypeFromAsClause eventSyntax.AsClause
        AccessModifiers = getMemberAccessModifiers eventSyntax.Modifiers defaultAccessModifier
        Modifiers = getAdditionalModifiers eventSyntax.Modifiers }

    let getMethod (methodSyntax : MethodStatementSyntax) defaultAccessModifier =
        let identity = {
            (createTypeIdentity methodSyntax.Parent) with
                Name = SyntaxHelper.getName methodSyntax.Identifier 0
                AccessModifiers = getMemberAccessModifiers methodSyntax.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers methodSyntax.Modifiers
                Parameters = getGenericParameterDefinitions methodSyntax.TypeParameterList }
        {   Identity = identity
            Parameters = getDelegateParameters methodSyntax.ParameterList
            ReturnType = getReturnType methodSyntax.SubOrFunctionKeyword methodSyntax.AsClause }
    
    let getMembers (structureSyntax : TypeBlockSyntax) defaultAccessModifiers =
        structureSyntax.Members
        |> Seq.fold (fun acc c -> 
            match c with
            | IsField x ->
                let fieldsDefinition = getFields x defaultAccessModifiers
                { acc with StructureMembers.Fields = List.append fieldsDefinition acc.Fields }
            | IsProperty _ ->
                let property = 
                    match c with
                    | IsPropertyBlock x -> Some <| getPropertyFromBlock x defaultAccessModifiers
                    | IsPropertyStatement x -> Some <| getPropertyFromStatement x defaultAccessModifiers
                    | _ -> None
                match property with
                | Some x -> { acc with Properties = x::acc.Properties }
                | None -> acc
            | IsMethod x ->
                let methodDefinition = getMethod x defaultAccessModifiers
                { acc with Methods = methodDefinition::acc.Methods }
            | IsEvent x ->
                let eventDefinition = getEvent x defaultAccessModifiers
                { acc with Events = eventDefinition::acc.Events }
            | _ -> acc) { Fields = []; Properties = []; Methods = []; Events = [] }

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
        let members = getMembers structureSyntax defaultAccessModifier
        {   Identity = identity
            Fields = members.Fields
            Properties = members.Properties
            Events = members.Events 
            Methods = members.Methods }

    let create structureSyntax =
        let definition = createDefinition structureSyntax
        match structureSyntax with
        | IsStruct _ -> Struct definition
        | IsInterface _ -> Interface definition
        | _ -> Class definition

module NamespaceDefinitionFactory =
    let create (namespaceSyntax : NamespaceBlockSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.NamespaceStatement.Name.ToString() }