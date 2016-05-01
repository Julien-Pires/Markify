module RoslynInspector_InspectGenericParameters
    open System
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open Inspectors

    open Xunit
    open Markify.Fixtures

    let filterValidGenericNode (node: SyntaxNode) =
        match node with
        | :? DelegateDeclarationSyntax -> true
        | :? BaseTypeDeclarationSyntax -> true
        | _ -> false

    let filterValidNamedNode (node: SyntaxNode) name = 
        match node with
        | :? DelegateDeclarationSyntax as d -> d.Identifier.Text = name
        | :? BaseTypeDeclarationSyntax as b -> b.Identifier.Text = name
        | _ -> false

    let getGenericNode (node: SyntaxNode) name =
        node.DescendantNodes()
        |> Seq.filter (filterValidGenericNode)
        |> Seq.find (fun c -> filterValidNamedNode c name)

    [<Fact>]
    let ``Inspect_WithNoGenericNode_ThrowException`` =
        let node = SyntaxFactory.IdentifierName ("Foo")

        Assert.Throws<ArgumentException> (fun () -> inspectGenerics node |> ignore);

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", 0, "SingleClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2, "")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericDelegate.cs", 1, "")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", 2, "")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericStruct.cs", 1, "")>]
    let ``Inspect_WhenTypeHasGenerics_WithExactParameters`` (count, name, tree: SyntaxTree) =
        let parameters = 
            (tree.GetRoot(), name)
            ||> getGenericNode
            |> inspectGenerics

        Assert.Equal (count, parameters.Length)