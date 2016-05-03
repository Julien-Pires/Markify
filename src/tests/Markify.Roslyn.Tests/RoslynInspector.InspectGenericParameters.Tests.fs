module RoslynInspector_InspectGenericParameters
    open System
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open Inspectors
    open Representation

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
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2, "GenericClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericDelegate.cs", 1, "Do")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", 2, "GenericInterface")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericStruct.cs", 1, "GenericStruct")>]
    let ``Inspect_WhenTypeHasGenerics_WithExactParameters`` (count, name, tree: SyntaxTree) =
        let parameters = 
            (tree.GetRoot(), name)
            ||> getGenericNode
            |> inspectGenerics

        Assert.Equal (count, parameters.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "T", "GenericClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "T", "GenericInterface")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "Y", "GenericInterface")>]
    let ``Inspect_WithCorrectName`` (parameterName, name, tree: SyntaxTree) =
        let parameter = 
            (tree.GetRoot(), name)
            ||> getGenericNode
            |> inspectGenerics
            |> Seq.tryFind(fun c -> toString c.Fullname = parameterName)

        Assert.True (parameter.IsSome)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "", "T", "GenericClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "out", "Y", "GenericInterface")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "in", "T", "GenericInterface")>]
    let ``Inspect_WhenHasModifier_WithExactModifiers`` (modifier: string, parameterName, name, tree: SyntaxTree) = 
        let optModifier = 
            match modifier with
            | "" -> None
            | _ -> Some(modifier)

        let parameter = 
            (tree.GetRoot(), name)
            ||> getGenericNode
            |> inspectGenerics
            |> Seq.tryFind(fun c -> toString c.Fullname = parameterName)

        Assert.True (parameter.IsSome)
        Assert.Equal (optModifier, parameter.Value.Modifier);

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2, "T", "GenericClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericDelegate.cs", 3, "T", "Do")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", 0, "T", "GenericInterface")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericStruct.cs", 1, "T", "GenericStruct")>]
    let ``Inspect_WhenHasConstraints_WithAllConstraints`` (count, parameterName, name, tree: SyntaxTree) =
        let parameter = 
            (tree.GetRoot(), name)
            ||> getGenericNode
            |> inspectGenerics
            |> Seq.tryFind(fun c -> toString c.Fullname = parameterName)

        Assert.True (parameter.IsSome)
        Assert.Equal(count, parameter.Value.Constraints.Length);