namespace Markify.Roslyn

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
        
open Markify.Models.Definitions
open VisualBasicSyntaxHelper
open Microsoft.CodeAnalysis.VisualBasic

module VisualBasicHelper =
    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.FriendKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

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

    let createTypeIdentity node =
        {   Name = ""
            Parents = SyntaxHelper.getParentName node getStructureTypeIdentifier 
            Namespace = SyntaxHelper.getNamespaceName node getNamespaceIdentifier
            AccessModifiers = []
            Modifiers = [] 
            BaseTypes = []
            Parameters = [] }

    let createStructureTypeDefinition (structureSyntax : TypeBlockSyntax) =
        let parameters = getGenericParameters structureSyntax.BlockStatement.TypeParameterList 
        let identity = {
            (createTypeIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.BlockStatement.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.BlockStatement.Modifiers 
                Modifiers = getAdditionalModifiers structureSyntax.BlockStatement.Modifiers 
                BaseTypes = getStructureTypeBaseTypes structureSyntax
                Parameters = getGenericParameterDefinitions parameters }
        match structureSyntax with
        | IsStruct _ -> Struct { Identity = identity }
        | IsInterface _ -> Interface { Identity = identity }
        | _ -> Class { Identity = identity }

    let createEnumDefinition (enumSyntax : EnumBlockSyntax) =
        let identity = {
            (createTypeIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.EnumStatement.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.EnumStatement.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.EnumStatement.Modifiers 
                BaseTypes = getEnumBaseType enumSyntax }
        Enum { Identity = identity }

    let createDelegateDefinition (delegateSyntax : DelegateStatementSyntax) =
        let parameters = getGenericParameters delegateSyntax.TypeParameterList 
        let identity = {
            (createTypeIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = getGenericParameterDefinitions parameters }
        Delegate { Identity = identity }

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