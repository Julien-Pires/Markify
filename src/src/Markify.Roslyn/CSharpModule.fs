namespace Markify.Roslyn

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

open CSharpSyntaxHelper
open Markify.Models.Definitions
open Microsoft.CodeAnalysis.CSharp

module CSharpHelper =
    let accessModifiersList =
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.InternalKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

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

    let createTypeIdentity node =
        {   Name = ""
            Parents = SyntaxHelper.getParentName node getStructureTypeIdentifier 
            Namespace = SyntaxHelper.getNamespaceName node getNamespaceIdentifier
            AccessModifiers = []
            Modifiers = [] 
            BaseTypes = []
            Parameters = [] }

    let createStructureTypeDefinition (structureSyntax : TypeDeclarationSyntax) =
        let parameters = getGenericParameters structureSyntax.TypeParameterList
        let identity = {
            (createTypeIdentity structureSyntax) with
                Name = SyntaxHelper.getName structureSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers structureSyntax.Modifiers 
                Modifiers = getAdditionalModifiers structureSyntax.Modifiers 
                BaseTypes = getBaseTypes structureSyntax 
                Parameters = getGenericParameterDefinitions parameters structureSyntax.ConstraintClauses }
        match structureSyntax with
        | IsStruct _ -> Struct { Identity = identity }
        | IsInterface _ -> Interface { Identity = identity }
        | _ -> Class { Identity = identity }

    let createEnumDefinition (enumSyntax : EnumDeclarationSyntax) =
        let identity = {
            (createTypeIdentity enumSyntax) with
                Name = SyntaxHelper.getName enumSyntax.Identifier 0
                AccessModifiers = getAccessModifiers enumSyntax.Modifiers 
                Modifiers = getAdditionalModifiers enumSyntax.Modifiers 
                BaseTypes = getBaseTypes enumSyntax }
        Enum { Identity = identity }

    let createDelegateDefinition (delegateSyntax : DelegateDeclarationSyntax) =
        let parameters = getGenericParameters delegateSyntax.TypeParameterList
        let identity = {
            (createTypeIdentity delegateSyntax) with
                Name = SyntaxHelper.getName delegateSyntax.Identifier (parameters |> Seq.length)
                AccessModifiers = getAccessModifiers delegateSyntax.Modifiers 
                Modifiers = getAdditionalModifiers delegateSyntax.Modifiers 
                Parameters = getGenericParameterDefinitions parameters delegateSyntax.ConstraintClauses }
        Delegate { Identity = identity }

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