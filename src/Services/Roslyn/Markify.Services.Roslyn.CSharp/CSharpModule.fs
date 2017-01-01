namespace Markify.Services.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax

module CSharpSyntaxHelper =
    let (|IsNamespace|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsClass|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as x -> Some x
        | _ -> None
        
    let (|IsInterface|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsStruct|_|) (node : SyntaxNode) =
        match node with
        | :? StructDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEnum|_|) (node : SyntaxNode) =
        match node with
        | :? EnumDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsDelegate|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsStructureType|_|) (node : SyntaxNode) = 
        match node with
        | IsClass x -> Some (x :> TypeDeclarationSyntax)
        | IsStruct x -> Some (x :> TypeDeclarationSyntax)
        | IsInterface x -> Some (x :> TypeDeclarationSyntax)
        | _ -> None

    let (|IsProperty|_|) (node : SyntaxNode) = 
        match node with
        | :? PropertyDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsField|_|) (node : SyntaxNode) =
        match node with
        | :? FieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsMethod|_|) (node : SyntaxNode) =
        match node with
        | :? MethodDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEventDeclaration|_|) (node : SyntaxNode) =
        match node with
        | :? EventDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsEventField|_|) (node : SyntaxNode) = 
        match node with
        | :? EventFieldDeclarationSyntax as x -> Some x
        | _ -> None

open CSharpSyntaxHelper
open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis.CSharp

module CSharpHelper =
    let publicModifier = [SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text]
    let privateModifier = [SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text]
    let accessModifiersList =
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.InternalKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

    let getDefaultMemberVisibility = function
        | IsInterface _ | IsEnum _ -> publicModifier
        | _ -> privateModifier

    let getStructureTypeIdentifier = function
        | IsStructureType x -> Some x.Identifier
        | _ -> None

    let getNamespaceIdentifier = function
        | IsNamespace x -> Some (x.Name :> SyntaxNode)
        | _ -> None

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

    let getMemberDefaultValue (initializer : EqualsValueClauseSyntax) =
        match initializer with
        | null -> None
        | x -> Some <| x.Value.ToString()

    let getBaseTypes (node : BaseTypeDeclarationSyntax) =
        match node.BaseList with
        | null -> []
        | x ->
            x.Types
            |> Seq.map (fun c -> c.Type.ToString())
            |> Seq.toList

    let getGenericParameters = function
        | null -> Seq.empty
        | (w : TypeParameterListSyntax) -> w.Parameters :> TypeParameterSyntax seq

    let getGenericParameterDefinitions (parameters : TypeParameterSyntax seq) (constraints : TypeParameterConstraintClauseSyntax seq) =
        let constraintsMap =
            constraints
            |> Seq.map (fun c ->
                let typesConstraints =
                    c.Constraints
                    |> Seq.map (fun d -> d.ToString())
                    |> Seq.toList
                (c.Name.ToString(), typesConstraints))
            |> Map.ofSeq
        parameters
        |> Seq.map (fun c ->
            let name = c.Identifier.Text
            let modifier = 
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some <| x.ToString()
            let constraints =
                constraintsMap
                |> Map.tryFind(name)
                |> function | Some x -> x | None -> []
            {   Name = name
                Modifier = modifier
                Constraints = constraints })
        |> Seq.toList

    let getMemberAccessModifiers modifiers defaultModifier =
        match getAccessModifiers modifiers with
        | [] -> defaultModifier
        | x -> x

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

    let createTypeIdentity node =
        {   Name = ""
            Parents = SyntaxHelper.getParentName node getStructureTypeIdentifier 
            Namespace = SyntaxHelper.getNamespaceName node getNamespaceIdentifier
            AccessModifiers = []
            Modifiers = [] 
            BaseTypes = []
            Parameters = [] }

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

    let getMethods (structureSyntax : TypeDeclarationSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsMethod x ->
                let parameters = getGenericParameters x.TypeParameterList
                let identity = {
                    (createTypeIdentity structureSyntax) with
                        Name = SyntaxHelper.getName x.Identifier 0
                        AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = getAdditionalModifiers x.Modifiers
                        Parameters = getGenericParameterDefinitions parameters x.ConstraintClauses  }
                let methodDefinition = {
                    Identity = identity
                    Parameters = getDelegateParameters x.ParameterList
                    ReturnType = x.ReturnType.ToString() }
                methodDefinition::acc
            | _ -> acc ) []

    let createStructureTypeDefinition (structureSyntax : TypeDeclarationSyntax) =
        let parameters = getGenericParameters structureSyntax.TypeParameterList
        let identity = {
            (createTypeIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.Modifiers 
                Modifiers = getAdditionalModifiers structureSyntax.Modifiers
                BaseTypes = getBaseTypes structureSyntax
                Parameters = getGenericParameterDefinitions parameters structureSyntax.ConstraintClauses }
        let defaultAccessModifier = getDefaultMemberVisibility structureSyntax
        let definition = {
            Identity = identity
            Fields = getFields structureSyntax defaultAccessModifier
            Properties = getProperties structureSyntax defaultAccessModifier
            Events = getEvents structureSyntax defaultAccessModifier 
            Methods = getMethods structureSyntax defaultAccessModifier }
        match structureSyntax with
        | IsStruct _ -> Struct definition
        | IsInterface _ -> Interface definition
        | _ -> Class definition

    let getEnumValues (enumSyntax : EnumDeclarationSyntax) =
        enumSyntax.Members
        |> Seq.map (fun c ->
            {   Name = c.Identifier.Text
                Value = getMemberDefaultValue c.EqualsValue })

    let createEnumDefinition (enumSyntax : EnumDeclarationSyntax) =
        let identity = {
            (createTypeIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.Modifiers 
                BaseTypes = getBaseTypes enumSyntax }
        Enum { 
            Identity = identity
            Values = getEnumValues enumSyntax }

    let createDelegateDefinition (delegateSyntax : DelegateDeclarationSyntax) =
        let genericParameters = getGenericParameters delegateSyntax.TypeParameterList
        let identity = {
            (createTypeIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (genericParameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = getGenericParameterDefinitions genericParameters delegateSyntax.ConstraintClauses }
        Delegate { 
            Identity = identity
            Parameters = getDelegateParameters delegateSyntax.ParameterList
            ReturnType = delegateSyntax.ReturnType.ToString() }

    let createNamespaceDefinition (namespaceSyntax : NamespaceDeclarationSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.Name.ToString() }

    let getDefinitions (root : SyntaxNode) =
        root.DescendantNodes()
        |> Seq.fold (fun (acc : SourceContent) c ->
            match c with
            | IsStructureType x ->
                let structureDefinition = createStructureTypeDefinition x
                { acc with Types = structureDefinition::acc.Types }
            | IsEnum x ->
                let enumDefinition = createEnumDefinition x
                { acc with Types = enumDefinition::acc.Types }
            | IsDelegate x ->
                let delegateDefinition = createDelegateDefinition x
                { acc with Types = delegateDefinition::acc.Types }
            | IsNamespace x -> 
                let namespaceDefinition = createNamespaceDefinition x
                { acc with Namespaces = namespaceDefinition::acc.Namespaces }
            | _ -> acc) { Types = []; Namespaces = [] }

module CSharpAnalyzer =
    let analyze (source : string) =
        let tree = CSharpSyntaxTree.ParseText source
        let root = tree.GetRoot()
        CSharpHelper.getDefinitions root