module Inspectors
    open TypeExtension
    open SyntaxNodeExtension
    open Markify.Models.Definitions

    open Microsoft.CodeAnalysis

    let getTypeKind (node : SyntaxNode) =
        match node with
        | ClassNode c -> StructureKind.Class
        | InterfaceNode i -> StructureKind.Interface
        | _ -> StructureKind.Unknown

    let searchTypes (|NodePattern|_|) (node : SyntaxNode) =
        node.DescendantNodes()
        |> Seq.filter(fun c -> 
            match c with
            | NodePattern n -> true
            | _ -> false)
        |> Seq.map(fun c ->
            {
                Identity = { Fullname = getFullname c; Name = (getName c).Value };
                Kind = getTypeKind c;
                AccessModifiers = getAccessModifiers c;
                Modifiers = getAdditionalModifiers c;
                Parameters = getGenericParameters c;
                BaseTypes = getBaseTypes c;
            })