namespace Markify.Services.Roslyn.Csharp

open DefinitionFactoryHelper
open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module DelegateDefinitionFactory =
    let create (delegateSyntax : DelegateDeclarationSyntax) =
        let parameters = getGenericParameterDefinitions delegateSyntax.TypeParameterList delegateSyntax.ConstraintClauses
        let identity = {
            (createIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = parameters }
        Delegate { 
            Identity = identity
            Parameters = getDelegateParameters delegateSyntax.ParameterList
            ReturnType = delegateSyntax.ReturnType.ToString() }

module EnumDefinitionFactory =
    let getEnumValues (enumSyntax : EnumDeclarationSyntax) =
        enumSyntax.Members
        |> Seq.map (fun c ->
            {   Name = c.Identifier.Text
                Value = getMemberDefaultValue c.EqualsValue })

    let create (enumSyntax : EnumDeclarationSyntax) =
        let identity = {
            (createIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.Modifiers
                Modifiers = getAdditionalModifiers enumSyntax.Modifiers 
                BaseTypes = getBaseTypes enumSyntax }
        Enum { 
            Identity = identity
            Values = getEnumValues enumSyntax }

module StructureDefinitionFactory =
    let getMemberAccessModifiers modifiers defaultModifier =
        match getAccessModifiers modifiers with
        | [] -> defaultModifier
        | x -> x

    let getFields (fieldSyntax : FieldDeclarationSyntax) defaultAccessModifier =
        fieldSyntax.Declaration.Variables
        |> Seq.fold (fun acc c ->
            let field = {
                FieldDefinition.Name = c.Identifier.Text
                Type = fieldSyntax.Declaration.Type.ToString()
                AccessModifiers = getMemberAccessModifiers fieldSyntax.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers fieldSyntax.Modifiers
                DefaultValue = getMemberDefaultValue c.Initializer }
            field::acc) []

    let getPropertyAccessors (propertySyntax : PropertyDeclarationSyntax) defaultAccessModifier =
        let getSingleAccessor (accessors : AccessorDeclarationSyntax seq) accessorType =
            accessors
            |> Seq.tryFind (fun c -> c.Keyword.Kind() = accessorType)
            |> function
                | None -> None
                | Some y ->
                    let accessModifiers = getMemberAccessModifiers y.Modifiers defaultAccessModifier
                    Some { AccessorDefinition.AccessModifiers = accessModifiers }
        match propertySyntax.AccessorList with
        | null -> (Some { AccessorDefinition.AccessModifiers = defaultAccessModifier }, None)
        | x -> (getSingleAccessor x.Accessors SyntaxKind.GetKeyword, getSingleAccessor x.Accessors SyntaxKind.SetKeyword)

    let getProperty (propertySyntax : PropertyDeclarationSyntax) defaultAccessModifier =
        let accessModifiers = getMemberAccessModifiers propertySyntax.Modifiers defaultAccessModifier
        let read, write = getPropertyAccessors propertySyntax accessModifiers
        {   Name = propertySyntax.Identifier.Text 
            AccessModifiers = accessModifiers
            Modifiers = getAdditionalModifiers propertySyntax.Modifiers
            Type = propertySyntax.Type.ToString()
            DefaultValue = getMemberDefaultValue propertySyntax.Initializer
            IsWrite = write
            IsRead = read }

    let getMethod (methodSyntax : MethodDeclarationSyntax) defaultAccessModifier =
        let identity = {
            (createIdentity methodSyntax.Parent) with
                Name = SyntaxHelper.getName methodSyntax.Identifier 0
                AccessModifiers = getMemberAccessModifiers methodSyntax.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers methodSyntax.Modifiers
                Parameters = getGenericParameterDefinitions methodSyntax.TypeParameterList methodSyntax.ConstraintClauses   }
        {   Identity = identity
            Parameters = getDelegateParameters methodSyntax.ParameterList
            ReturnType = methodSyntax.ReturnType.ToString() }

    let getEvents (eventSyntax : MemberDeclarationSyntax) defaultAccessModifier =
        match eventSyntax with
        | IsEventDeclaration x ->
            [{  EventDefinition.Name = x.Identifier.Text 
                Type = x.Type.ToString()
                AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers x.Modifiers }]
        | IsEventField x -> 
            let declaration = x.Declaration
            declaration.Variables
            |> Seq.fold (fun acc d -> 
                let event = { 
                    EventDefinition.Name = d.Identifier.Text 
                    Type = declaration.Type.ToString()
                    AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                    Modifiers = getAdditionalModifiers x.Modifiers }
                event::acc) []
        | _ -> []

    let getMembers (structureSyntax : TypeDeclarationSyntax) defaultAccessModifiers =
        let members = ([], [], [], [])
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsMethod x -> 
                let (m, p, f, e) = acc
                let methodDefinition = getMethod x defaultAccessModifiers
                (methodDefinition::m, p, f, e)
            | IsField x ->
                let (m, p, f, e) = acc
                let fieldsDefinition = getFields x defaultAccessModifiers
                (m, p, fieldsDefinition |> List.append f, e)
            | IsProperty x ->
                let (m, p, f, e) = acc
                let propertyDefinition = getProperty x defaultAccessModifiers
                (m, propertyDefinition::p, f, e)
            | IsEvent x ->
                let (m, p, f, e) = acc
                let eventsDefinition = getEvents x defaultAccessModifiers
                (m, p, f, eventsDefinition |> List.append e)
            | _ -> acc) members

    let createDefinition (structureSyntax : TypeDeclarationSyntax)=
        let parameters = getGenericParameterDefinitions structureSyntax.TypeParameterList structureSyntax.ConstraintClauses
        let identity = {
            (createIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.Modifiers
                Modifiers = getAdditionalModifiers structureSyntax.Modifiers 
                BaseTypes = getBaseTypes structureSyntax
                Parameters = parameters  }
        let defaultAccessModifier = getDefaultMemberVisibility structureSyntax
        let methods, properties, fields, events = getMembers structureSyntax defaultAccessModifier
        {   Identity = identity
            Fields = fields
            Properties = properties
            Events = events
            Methods = methods }

    let create structureSyntax =
        let definition = createDefinition structureSyntax
        match structureSyntax with
        | IsStruct _ -> Struct definition
        | IsInterface _ -> Interface definition
        | _ -> Class definition

module NamespaceDefinitionFactory =
    let create (namespaceSyntax : NamespaceDeclarationSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.Name.ToString() }