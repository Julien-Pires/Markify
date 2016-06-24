namespace Markify.Roslyn

open TypeExtension
open SyntaxNodeExtension

open Markify.Models.Definitions

open Microsoft.CodeAnalysis

module Inspector =
    let searchTypes (node : SyntaxNode) =
        node.DescendantNodes()
        |> Seq.filter(fun c -> 
            match c with
            | TypeNode _ -> true
            | _ -> false)
        |> Seq.map(fun c ->
            {
                Identity = { Fullname = getFullname c; Name = (getName c).Value };
                Kind = getTypeKind c;
                AccessModifiers = getAccessModifiers c;
                Modifiers = getAdditionalModifiers c;
                Parameters = getGenericParameters c;
                BaseTypes = getBaseTypes c; })