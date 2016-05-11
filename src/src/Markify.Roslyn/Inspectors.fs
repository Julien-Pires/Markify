module Inspectors
    open Container
    open Representation
    open TypeExtension
    open SyntaxNodeExtension

    open Microsoft.CodeAnalysis

    let inspectClass (node: SyntaxNode) : StructureContainer list =
        let f = 
            node.DescendantNodes()
            |> Seq.filter(fun c -> 
                match c with
                | TypeNode t -> true
                | _ -> false)
            |> Seq.map(fun c ->
                {
                    Fullname = fullname c;
                    Kind = StructureKind.Class;
                    AccessModifiers = accessModifiers c;
                    AdditionalModifiers = additionalModifiers c;
                    GenericParameters = [];
                    BaseTypes = baseTypes c;
                })
            |> Seq.map(fun c -> { Representation = c })

        Seq.toList f