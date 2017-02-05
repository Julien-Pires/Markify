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

    let getFields (structureSyntax : TypeDeclarationSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold(fun acc c ->
            match c with
            | IsField x -> 
                x.Declaration.Variables
                |> Seq.fold (fun acc2 d ->
                    let field = {
                        FieldDefinition.Name = d.Identifier.Text
                        Type = x.Declaration.Type.ToString()
                        AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = getAdditionalModifiers x.Modifiers
                        DefaultValue = getMemberDefaultValue d.Initializer }
                    field::acc2) acc
            | _ -> acc) []

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

    let getProperties (structureSyntax : TypeDeclarationSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsProperty x ->
                let accessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                let read, write = getPropertyAccessors x accessModifiers
                let property = {
                    Name = x.Identifier.Text 
                    AccessModifiers = accessModifiers
                    Modifiers = getAdditionalModifiers x.Modifiers
                    Type = x.Type.ToString()
                    DefaultValue = getMemberDefaultValue x.Initializer
                    IsWrite = write
                    IsRead = read }
                property::acc
            | _ -> acc) []

    let getMethods (structureSyntax : TypeDeclarationSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsMethod x ->
                let identity = {
                    (createIdentity structureSyntax) with
                        Name = SyntaxHelper.getName x.Identifier 0
                        AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = getAdditionalModifiers x.Modifiers
                        Parameters = getGenericParameterDefinitions x.TypeParameterList x.ConstraintClauses   }
                let methodDefinition = {
                    Identity = identity
                    Parameters = getDelegateParameters x.ParameterList
                    ReturnType = x.ReturnType.ToString() }
                methodDefinition::acc
            | _ -> acc ) []

    let getEvents (structureSyntax : TypeDeclarationSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c -> 
            match c with
            | IsEventDeclaration x ->
                let event = {
                    EventDefinition.Name = x.Identifier.Text 
                    Type = x.Type.ToString()
                    AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                    Modifiers = getAdditionalModifiers x.Modifiers }
                event::acc
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
                |> List.append acc
            | _ -> acc) []

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
    let create (namespaceSyntax : NamespaceDeclarationSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.Name.ToString() }