namespace Markify.Services.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module VisualBasicSyntaxHelper =
    let (|IsNamespace|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsClass|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassBlockSyntax as x -> Some x
        | _ -> None

    let (|IsInterface|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceBlockSyntax as x -> Some x
        | _ -> None

    let (|IsStruct|_|) (node : SyntaxNode) =
        match node with
        | :? StructureBlockSyntax as x -> Some x
        | _ -> None

    let (|IsEnum|_|) (node : SyntaxNode) =
        match node with
        | :? EnumBlockSyntax as x -> Some x
        | _ -> None

    let (|IsDelegate|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateStatementSyntax as x -> Some x
        | _ -> None

    let (|IsStructureType|_|) (node : SyntaxNode) =
        match node with
        | IsClass x -> Some (x :> TypeBlockSyntax)
        | IsStruct x -> Some (x :> TypeBlockSyntax)
        | IsInterface x -> Some (x :> TypeBlockSyntax)
        | _ -> None

    let (|IsPropertyStatement|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyStatementSyntax as x -> Some x
        | _ -> None

    let (|IsPropertyBlock|_|) (node : SyntaxNode) =
        match node with
        | :? PropertyBlockSyntax as x -> Some x
        | _ -> None

    let (|IsField|_|) (node : SyntaxNode) =
        match node with
        | :? FieldDeclarationSyntax as x -> Some x
        | _ -> None

    let (|IsMethod|_|) (node : SyntaxNode) =
        match node with
        | :? MethodStatementSyntax as x -> Some x
        | :? MethodBlockSyntax as x -> Some (x.BlockStatement :?> MethodStatementSyntax)
        | _ -> None

    let (|IsEventStatement|_|) (node : SyntaxNode) =
        match node with
        | :? EventStatementSyntax as x -> Some x
        | _ -> None

    let (|IsEventBlock|_|) (node : SyntaxNode) =
        match node with
        | :? EventBlockSyntax as x -> Some x
        | _ -> None
        
open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common
open VisualBasicSyntaxHelper
open Microsoft.CodeAnalysis.VisualBasic

module VisualBasicHelper =
    let publicModifier = [SyntaxFactory.Token(SyntaxKind.PublicKeyword).Text]
    let privateModifier = [SyntaxFactory.Token(SyntaxKind.PrivateKeyword).Text]
    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.FriendKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

    let getDefaultMemberVisibility = function
        | IsInterface _ | IsEnum _ -> publicModifier
        | _ -> privateModifier

    let getStructureTypeIdentifier = function
        | IsStructureType x -> Some x.BlockStatement.Identifier
        | _ -> None

    let getNamespaceIdentifier = function
        | IsNamespace x -> Some (x.NamespaceStatement.Name :> SyntaxNode)
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

    let containsModifiers (modifiers : SyntaxTokenList) modifier =
        modifiers
        |> Seq.exists (fun c -> c.Kind() = modifier)

    let getTypeFromAsClause (clause : AsClauseSyntax) =
        match clause with
        | null -> "Object"
        | _ -> clause.Type().ToString()

    let getReturnType (delegateType : SyntaxToken) clause =
        match delegateType.Kind() with
        | SyntaxKind.SubKeyword -> "Void"
        | _ -> getTypeFromAsClause clause

    let getMemberDefaultValue (initializer : EqualsValueSyntax) =
        match initializer with
        | null -> None
        | x -> Some <| x.Value.ToString()

    let getStructureTypeBaseTypes (node : TypeBlockSyntax) =
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

    let getEnumBaseType (node : EnumBlockSyntax) =
        match node.EnumStatement.UnderlyingType with
        | null -> []
        | x -> [x.Type().ToString()]

    let getGenericParameters = function
        | null -> Seq.empty
        | (w : TypeParameterListSyntax) -> w.Parameters :> TypeParameterSyntax seq

    let getGenericParameterDefinitions (parameters : TypeParameterSyntax seq) =
        parameters
        |> Seq.map (fun c ->
            let constraints =
                match c.TypeParameterConstraintClause with
                | :? TypeParameterSingleConstraintClauseSyntax as x -> SeparatedSyntaxList().Add x.Constraint
                | :? TypeParameterMultipleConstraintClauseSyntax as x -> x.Constraints
                | null | _ -> SeparatedSyntaxList()
            let modifier =
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some <| x.ToString()
            {   Name = c.Identifier.Text
                Modifier = modifier
                Constraints = constraints |> Seq.map (fun c -> c.ToString()) |> Seq.toList })
        |> Seq.toList

    let getMemberAccessModifiers modifiers defaultModifier =
        match getAccessModifiers modifiers with
        | [] -> defaultModifier
        | x -> x :> Modifier seq

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
            {   ParameterDefinition.Name = c.Identifier.Identifier.Text 
                Type = getTypeFromAsClause c.AsClause
                Modifier = (getAdditionalModifiers c.Modifiers) |> List.tryHead 
                DefaultValue = getMemberDefaultValue c.Default })
        |> Seq.toList

    let getMethods (structureSyntax : TypeBlockSyntax) defaultAccessModifier =
        structureSyntax.Members
        |> Seq.fold (fun acc c ->
            match c with
            | IsMethod x ->
                let parameters = getGenericParameters x.TypeParameterList
                let identity = {
                    (createTypeIdentity x) with
                        Name = SyntaxHelper.getName x.Identifier 0
                        AccessModifiers = getMemberAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = getAdditionalModifiers x.Modifiers
                        Parameters = getGenericParameterDefinitions parameters }
                let methodDefinition = {
                    Identity = identity
                    Parameters = getDelegateParameters x.ParameterList
                    ReturnType = getReturnType x.SubOrFunctionKeyword x.AsClause }
                methodDefinition::acc
            | _ -> acc ) []

    let createStructureTypeDefinition (structureSyntax : TypeBlockSyntax) =
        let parameters = getGenericParameters structureSyntax.BlockStatement.TypeParameterList
        let identity = {
            (createTypeIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.BlockStatement.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.BlockStatement.Modifiers 
                Modifiers = getAdditionalModifiers structureSyntax.BlockStatement.Modifiers 
                BaseTypes = getStructureTypeBaseTypes structureSyntax
                Parameters = getGenericParameterDefinitions parameters }
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

    let getEnumValues (enumSyntax : EnumBlockSyntax) =
        enumSyntax.Members
        |> Seq.map (fun c -> 
            let enumValue = c :?> EnumMemberDeclarationSyntax
            {   Name = enumValue.Identifier.Text
                Value = getMemberDefaultValue enumValue.Initializer })

    let createEnumDefinition (enumSyntax : EnumBlockSyntax) =
        let identity = {
            (createTypeIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.EnumStatement.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.EnumStatement.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.EnumStatement.Modifiers 
                BaseTypes = getEnumBaseType enumSyntax }
        Enum { 
            Identity = identity
            Values = getEnumValues enumSyntax}

    let createDelegateDefinition (delegateSyntax : DelegateStatementSyntax) =
        let genericParameters = getGenericParameters delegateSyntax.TypeParameterList 
        let identity = {
            (createTypeIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (genericParameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = getGenericParameterDefinitions genericParameters }
        Delegate { 
            Identity = identity 
            Parameters = getDelegateParameters delegateSyntax.ParameterList
            ReturnType = getReturnType delegateSyntax.SubOrFunctionKeyword delegateSyntax.AsClause }

    let createNamespaceDefinition (namespaceSyntax : NamespaceBlockSyntax) =
        { NamespaceDefinition.Name = namespaceSyntax.NamespaceStatement.Name.ToString() }

    let getDefinitions (root : SyntaxNode) =
        root.DescendantNodes()
        |> Seq.fold (fun (acc :SourceContent) c ->
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

module VisualBasicAnalyzer =
    let analyze (source : string) =
        let tree = VisualBasicSyntaxTree.ParseText source
        let root = tree.GetRoot()
        VisualBasicHelper.getDefinitions root