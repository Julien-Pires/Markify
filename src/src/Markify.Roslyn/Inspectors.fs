module Inspectors
    open Container
    open Representation
    open RoslynExtension

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp.Syntax

    let inspectClass (node: SyntaxNode) : StructureContainer list =
        let f = 
            node.DescendantNodes()
            |> Seq.filter(fun c -> 
                match c with
                | :? ClassDeclarationSyntax -> true
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

    let inspectGenerics node : GenericParametersList = Seq.empty<GenericParameterRepresentation>