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
                Identity = { Fullname = fullname c; Name = (name c).Value };
                Kind = StructureKind.Class;
                AccessModifiers = accessModifiers c;
                Modifiers = additionalModifiers c;
                Parameters = getGenericParameters c;
                BaseTypes = baseTypes c;
            })