namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open DefinitionFactoryHelper
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module DelegateDefinitionFactory =
    let buildDefinition (delegateSyntax : DelegateStatementSyntax) =
        let definition = {   
            Identity = createIdentity delegateSyntax
            Comments = { Comments = [] }
            Parameters = []
            ReturnType = getReturnType delegateSyntax.SubOrFunctionKeyword delegateSyntax.AsClause }
        (delegateSyntax, definition)
    
    let buildIdentity (delegateSyntax : DelegateStatementSyntax, definition : DelegateDefinition) =
        let parameters = getGenericParameterDefinitions delegateSyntax.TypeParameterList
        let identity = {
            (definition.Identity) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = parameters }
        (delegateSyntax, { definition with Identity = identity })
    
    let buildParameters (delegateSyntax : DelegateStatementSyntax, definition : DelegateDefinition) =
        let parameters = getDelegateParameters delegateSyntax.ParameterList
        ( delegateSyntax, { definition with Parameters = parameters })

    let buildComments (delegateSyntax, definition : DelegateDefinition) =
        (delegateSyntax, { definition with Comments = getTypeComments delegateSyntax })

    let create (delegateSyntax : DelegateStatementSyntax) =
        let definition = 
            delegateSyntax
            |> buildDefinition
            |> buildIdentity
            |> buildComments
            |> buildParameters
        definition |> snd |> Delegate

module EnumDefinitionFactory =
    let buildDefinition (enumSyntax : EnumBlockSyntax) =
        let definition = {   
            Identity = createIdentity enumSyntax
            Comments = { Comments = [] }
            Values = [] }
        (enumSyntax, definition)

    let getBaseType (node : EnumBlockSyntax) =
        match node.EnumStatement.UnderlyingType with
        | null -> []
        | x -> [x.Type().ToString()]

    let buildIdentity (enumSyntax : EnumBlockSyntax, definition : EnumDefinition) =
        let identity = {
            (definition.Identity) with
                Name = SyntaxHelper.getName enumSyntax.EnumStatement.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.EnumStatement.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.EnumStatement.Modifiers 
                BaseTypes = getBaseType enumSyntax }
        (enumSyntax, { definition with Identity = identity })

    let buildValues (enumSyntax : EnumBlockSyntax, definition : EnumDefinition) =
        let values =
            enumSyntax.Members
            |> Seq.map (fun c ->
                let enumValue = c :?> EnumMemberDeclarationSyntax
                {   EnumValue.Name = enumValue.Identifier.Text
                    Value = getMemberDefaultValue enumValue.Initializer })
        (enumSyntax, { definition with Values = values })

    let buildComments (enumSyntax : EnumBlockSyntax, definition : EnumDefinition) =
        (enumSyntax, { definition with Comments = getTypeComments enumSyntax })

    let create enumSyntax =
        let definition =
            enumSyntax
            |> buildDefinition
            |> buildIdentity
            |> buildComments
            |> buildValues
        definition |> snd |> Enum

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
                    FieldInfo.Name = d.Identifier.Text
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
            PropertyInfo.Name = propertyStatement.Identifier.Text 
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
        EventInfo.Name = eventSyntax.Identifier.Text 
        Type = getTypeFromAsClause eventSyntax.AsClause
        AccessModifiers = getMemberAccessModifiers eventSyntax.Modifiers defaultAccessModifier
        Modifiers = getAdditionalModifiers eventSyntax.Modifiers }

    let getMethod (methodSyntax : MethodStatementSyntax) defaultAccessModifier =
        let identity = {
            (createIdentity methodSyntax.Parent) with
                Name = SyntaxHelper.getName methodSyntax.Identifier 0
                AccessModifiers = getMemberAccessModifiers methodSyntax.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers methodSyntax.Modifiers
                Parameters = getGenericParameterDefinitions methodSyntax.TypeParameterList }
        {   Identity = identity
            Comments = { Comments = [] }
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

    let buildDefinition (structureSyntax : TypeBlockSyntax) =
        let definition = {   
            Identity = createIdentity structureSyntax
            Comments = { Comments = [] }
            Fields = []
            Properties = []
            Events = []
            Methods = [] }
        (structureSyntax, definition)

    let buildIdentity (structureSyntax : TypeBlockSyntax, definition : ClassDefinition) =
        let parameters = getGenericParameterDefinitions structureSyntax.BlockStatement.TypeParameterList
        let identity = {
            (createIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.BlockStatement.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.BlockStatement.Modifiers
                Modifiers = getAdditionalModifiers structureSyntax.BlockStatement.Modifiers 
                BaseTypes = getBaseType structureSyntax
                Parameters = parameters }
        (structureSyntax, { definition with Identity = identity })

    let buildMembers (structureSyntax : TypeBlockSyntax, definition : ClassDefinition) =
        let defaultAccessModifier = getDefaultMemberVisibility structureSyntax
        let members = getMembers structureSyntax defaultAccessModifier
        let def = {   
            definition with
                Fields = members.Fields
                Properties = members.Properties
                Events = members.Events
                Methods = members.Methods }
        (structureSyntax, def)
    
    let buildComments (structureSyntax : TypeBlockSyntax, definition : ClassDefinition) =
        let newDefinition = { definition with Comments = getTypeComments structureSyntax }
        (structureSyntax, newDefinition)

    let build structureSyntax = 
        structureSyntax 
        |> buildDefinition 
        |> buildIdentity 
        |> buildMembers
        |> buildComments

    let create structureSyntax =
        let definition = build structureSyntax |> snd
        match structureSyntax with
        | IsStruct _ -> Struct definition
        | IsInterface _ -> Interface definition
        | _ -> Class definition

module NamespaceDefinitionFactory =
    let create (namespaceSyntax : NamespaceBlockSyntax) =
        { NamespaceInfo.Name = namespaceSyntax.NamespaceStatement.Name.ToString() }