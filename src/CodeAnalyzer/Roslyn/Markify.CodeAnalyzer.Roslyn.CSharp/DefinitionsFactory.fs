namespace Markify.CodeAnalyzer.Roslyn.Csharp

open DefinitionFactoryHelper
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn.Common
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module DelegateDefinitionFactory =
    let buildDefinition (delegateSyntax : DelegateDeclarationSyntax) =
        let definition = {   
            Identity = createIdentity delegateSyntax
            Comments = { Comments = [] }
            Parameters = []
            ReturnType = delegateSyntax.ReturnType.ToString() }
        (delegateSyntax, definition)
    
    let buildIdentity (delegateSyntax : DelegateDeclarationSyntax, definition : DelegateDefinition) =
        let parameters = getGenericParameterDefinitions delegateSyntax.TypeParameterList delegateSyntax.ConstraintClauses
        let identity = {
            (definition.Identity) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = parameters }
        (delegateSyntax, { definition with Identity = identity })
    
    let buildParameters (delegateSyntax : DelegateDeclarationSyntax, definition : DelegateDefinition) =
        let parameters = getDelegateParameters delegateSyntax.ParameterList
        ( delegateSyntax, { definition with Parameters = parameters })
  
    let buildComments (delegateSyntax : DelegateDeclarationSyntax, definition : DelegateDefinition) =
        (delegateSyntax, { definition with Comments = getTypeComments delegateSyntax })

    let create (delegateSyntax : DelegateDeclarationSyntax) =
        let definition = 
            delegateSyntax
            |> buildDefinition
            |> buildIdentity
            |> buildComments
            |> buildParameters
        definition |> snd |> Delegate

module EnumDefinitionFactory =    
    let buildDefinition (enumSyntax : EnumDeclarationSyntax) =
        let definition = {   
            Identity = createIdentity enumSyntax
            Comments = { Comments = [] }
            Values = [] }
        (enumSyntax, definition)

    let buildIdentity (enumSyntax : EnumDeclarationSyntax, definition : EnumDefinition) =
        let identity = {
            (definition.Identity) with
                Name = SyntaxHelper.getName enumSyntax.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.Modifiers
                Modifiers = getAdditionalModifiers enumSyntax.Modifiers 
                BaseTypes = getBaseTypes enumSyntax }
        (enumSyntax, { definition with Identity = identity })

    let buildValues (enumSyntax : EnumDeclarationSyntax, definition : EnumDefinition) =
        let values =
            enumSyntax.Members
            |> Seq.map (fun c ->
                {   EnumValue.Name = c.Identifier.Text
                    Value = getMemberDefaultValue c.EqualsValue })
        (enumSyntax, { definition with Values = values })

    let buildComments (enumSyntax : EnumDeclarationSyntax, definition : EnumDefinition) =
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
    let getMemberAccessModifiers modifiers defaultModifier =
        match getAccessModifiers modifiers with
        | [] -> defaultModifier
        | x -> x

    let getFields (fieldSyntax : FieldDeclarationSyntax) defaultAccessModifier =
        fieldSyntax.Declaration.Variables
        |> Seq.fold (fun acc c ->
            let field = {
                FieldInfo.Name = c.Identifier.Text
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
                    Some { AccessorInfo.AccessModifiers = accessModifiers }
        match propertySyntax.AccessorList with
        | null -> (Some { AccessorInfo.AccessModifiers = defaultAccessModifier }, None)
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
            Comments = { Comments = [] }
            Parameters = getDelegateParameters methodSyntax.ParameterList
            ReturnType = methodSyntax.ReturnType.ToString() }

    let getEvents (eventSyntax : MemberDeclarationSyntax) defaultAccessModifier =
        match eventSyntax with
        | IsEventDeclaration x ->
            [{  EventInfo.Name = x.Identifier.Text 
                Type = x.Type.ToString()
                AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                Modifiers = getAdditionalModifiers x.Modifiers }]
        | IsEventField x -> 
            let declaration = x.Declaration
            declaration.Variables
            |> Seq.fold (fun acc d -> 
                let event = { 
                    EventInfo.Name = d.Identifier.Text 
                    Type = declaration.Type.ToString()
                    AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                    Modifiers = getAdditionalModifiers x.Modifiers }
                event::acc) []
        | _ -> []

    let getMembers (structureSyntax : TypeDeclarationSyntax) defaultAccessModifiers =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsField x ->
                let fieldsDefinition = getFields x defaultAccessModifiers
                { acc with StructureMembers.Fields = List.append fieldsDefinition acc.Fields }
            | IsProperty x ->
                let propertyDefinition = getProperty x defaultAccessModifiers
                { acc with Properties = propertyDefinition::acc.Properties }
            | IsMethod x ->
                let methodDefinition = getMethod x defaultAccessModifiers
                { acc with Methods = methodDefinition::acc.Methods }
            | IsEvent x ->
                let eventsDefinition = getEvents x defaultAccessModifiers
                { acc with Events = List.append eventsDefinition acc.Events }
            | _ -> acc) { Fields = []; Properties = []; Methods = []; Events = [] }
    
    let buildDefinition (structureSyntax : TypeDeclarationSyntax) =
        let definition = {   
            Identity = createIdentity structureSyntax
            Comments = { Comments = [] }
            Fields = []
            Properties = []
            Events = []
            Methods = [] }
        (structureSyntax, definition)

    let buildIdentity (structureSyntax : TypeDeclarationSyntax, definition : ClassDefinition) =
        let parameters = getGenericParameterDefinitions structureSyntax.TypeParameterList structureSyntax.ConstraintClauses
        let identity = {
            (createIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.Modifiers
                Modifiers = getAdditionalModifiers structureSyntax.Modifiers 
                BaseTypes = getBaseTypes structureSyntax
                Parameters = parameters }
        (structureSyntax, { definition with Identity = identity })

    let buildMembers (structureSyntax : TypeDeclarationSyntax, definition : ClassDefinition) =
        let defaultAccessModifier = getDefaultMemberVisibility structureSyntax
        let members = getMembers structureSyntax defaultAccessModifier
        let def = {   
            definition with
                Fields = members.Fields
                Properties = members.Properties
                Events = members.Events
                Methods = members.Methods }
        (structureSyntax, def)
    
    let buildComments (structureSyntax : TypeDeclarationSyntax, definition : ClassDefinition) =
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
    let create (namespaceSyntax : NamespaceDeclarationSyntax) =
        { NamespaceInfo.Name = namespaceSyntax.Name.ToString() }