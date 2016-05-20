module Inspectors
    open TypeExtension
    open SyntaxNodeExtension
    open Markify.Models.Definitions

    open Microsoft.CodeAnalysis

    let searchTypes (node: SyntaxNode) =
        node.DescendantNodes()
        |> Seq.filter(fun c -> 
            match c with
            | TypeNode t -> true
            | _ -> false)
        |> Seq.map(fun c ->
            {
                Identity = { Fullname = getFullname c; Name = (getName c).Value };
                Kind = StructureKind.Class;
                AccessModifiers = getAccessModifiers c;
                Modifiers = getAdditionalModifiers c;
                Parameters = getGenericParameters c;
                BaseTypes = getBaseTypes c;
            })