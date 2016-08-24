namespace Markify.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax

module CSharpSyntaxHelper =
    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassDeclarationSyntax as x -> Some x
        | _ -> None
        
    let (|InterfaceNode|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceDeclarationSyntax as x -> Some x
        | _ -> None

    let (|StructNode|_|) (node : SyntaxNode) =
        match node with
        | :? StructDeclarationSyntax as x -> Some x
        | _ -> None

    let (|EnumNode|_|) (node : SyntaxNode) =
        match node with
        | :? EnumDeclarationSyntax as x -> Some x
        | _ -> None

    let (|DelegateNode|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax as x -> Some x
        | _ -> None

    let (|ObjectNode|_|) (node : SyntaxNode) =
        match node with
        | :? BaseTypeDeclarationSyntax as x -> Some x
        | _ -> None

    let (|ContainerTypeNode|_|) (node : SyntaxNode) = 
        match node with
        | :? TypeDeclarationSyntax as x -> Some x
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

type CSharpHelper() =
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

    let getGenericParameters node =
        let parametersList =
            match node with
            | ContainerTypeNode x -> Some x.TypeParameterList
            | DelegateNode x -> Some x.TypeParameterList
            | _ -> None
        match parametersList with
        | None -> SeparatedSyntaxList()
        | Some x ->
            match x with
            | null -> SeparatedSyntaxList()
            | w -> w.Parameters

    override this.ReadSource source =
        CSharpSyntaxTree.ParseText source

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
        | ObjectNode x ->
            match x.BaseList with
            | null -> Seq.empty
            | w ->
                w.Types
                |> Seq.map (fun c -> c.Type.ToString())
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
                |> Seq.map (fun d -> d.ToString())
            let constraints = {
                TypeConstraint.Name = c.Name.ToString()
                Constraints = typesConstraints}
            constraints)

    override this.GetGenericParameters node =
        getGenericParameters node
        |> Seq.map (fun c -> 
            let name = c.Identifier.Text
            let modifier = 
                match c.VarianceKeyword.Value with
                | null -> ""
                | x -> x.ToString()
            let parameter = {
                Name = name
                Modifier = modifier}
            parameter)

    override this.IsTypeNode node =
        (|TypeNode|_|) node

    override this.IsNamespaceNode node =
        match node with
        | NamespaceNode _ -> Some true
        | _ -> None