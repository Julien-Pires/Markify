namespace Markify.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax

module CSharpSyntaxHelper =
    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as c -> Some c
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as c -> Some c
        | _ -> None
        
    let (|InterfaceNode|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceDeclarationSyntax as c -> Some c
        | _ -> None

    let (|StructNode|_|) (node : SyntaxNode) =
        match node with
        | :? StructDeclarationSyntax as c -> Some c
        | _ -> None

    let (|EnumNode|_|) (node : SyntaxNode) =
        match node with
        | :? EnumDeclarationSyntax as c -> Some c
        | _ -> None

    let (|DelegateNode|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax as c -> Some c
        | _ -> None

    let (|ObjectNode|_|) (node : SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as c -> Some c
        | _ -> None

    let (|ContainerTypeNode|_|) (node : SyntaxNode) = 
        match node with
        | :? TypeDeclarationSyntax as c -> Some c
        | _ -> None

    let (|TypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ClassNode x -> Some true
        | InterfaceNode x -> Some true
        | StructNode x -> Some true
        | EnumNode x -> Some true
        | DelegateNode x -> Some true
        | _ -> None

open CSharpSyntaxHelper
open Markify.Models.Definitions
open Microsoft.CodeAnalysis.CSharp

type CSharpNodeHelper() =
    inherit NodeHelper()

    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.InternalKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

    let getModifiers filter (node : SyntaxNode) =
        let modifiers =
            match node with
            | ObjectNode x -> x.Modifiers
            | DelegateNode x -> x.Modifiers
            | _ -> SyntaxTokenList()
        modifiers
        |> Seq.filter filter
        |> Seq.map (fun c -> c.Text)

    override this.GetTypeName node =
        match node with
        | ObjectNode x -> x.Identifier.Text
        | DelegateNode x -> x.Identifier.Text
        | _ -> ""

    override this.GetNamespaceName node =
        match node with
        | NamespaceNode x -> x.Name.ToString()
        | _ -> ""

    override this.GetTypeKind node =
        match node with
        | ClassNode _ -> StructureKind.Class
        | InterfaceNode _ -> StructureKind.Interface
        | StructNode _ -> StructureKind.Struct
        | EnumNode _ -> StructureKind.Enum
        | DelegateNode _ -> StructureKind.Delegate
        | _ -> StructureKind.Unknown

    override this.GetModifiers node =
        node
        |> getModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind())
            |> not)

    override this.GetAccessModifiers node =
        node
        |> getModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind()))

    override this.GetParents node =
        match node with
        | ObjectNode x -> LanguageHelper.getBaseTypes x
        | _ -> Seq.empty

    override this.GetGenericConstraints node =
        let constraintsList =
            match node with
            | ContainerTypeNode x -> x.ConstraintClauses :> TypeParameterConstraintClauseSyntax seq
            | DelegateNode x -> x.ConstraintClauses :> TypeParameterConstraintClauseSyntax seq
            | _ -> Seq.empty
        constraintsList
        |> Seq.map (fun c ->
            let typesConstraints = 
                c.Constraints
                |> Seq.map (fun c -> c.ToString())
            let constraints = {
                TypeConstraints.Name = c.Name.ToString()
                Constraints = typesConstraints}
            constraints)

    override this.GetGenericParameters node =
        let parameters =
            match node with
            | ContainerTypeNode x -> LanguageHelper.getGenericParameters x
            | DelegateNode x -> LanguageHelper.getGenericParameters x
            | _ -> Seq.empty
        parameters

module CSharpModule = 
    open Markify.Roslyn.LanguageHelper
    open Microsoft.CodeAnalysis.CSharp

    let nodeHelper = CSharpNodeHelper()
    let nodeFactory = NodeFactory(nodeHelper)
    
    let rec getNode node =
        match node with
        | TypeNode _ -> nodeFactory.buildTypeNode node getNode
        | NamespaceNode x -> nodeFactory.buildNamespaceNode x
        | null -> NoNode
        | _ -> nodeFactory.buildOtherNode node getNode

    let readSource (source : string) =
        CSharpSyntaxTree.ParseText source

    let inspect source =
        let tree = readSource source
        let root = tree.GetRoot()
        root.DescendantNodes()
        |> Seq.filter (fun c -> 
            match c with
            | TypeNode _ -> true
            | _ -> false)
        |> Seq.map getNode