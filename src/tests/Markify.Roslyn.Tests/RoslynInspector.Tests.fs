module RoslynInspector
    open Xunit
    open Representation
    open Markify.Fixtures
    open Microsoft.CodeAnalysis
    open Inspectors
    open Ploeh.AutoFixture.Xunit2

    [<Theory>]
    [<SyntaxTreeInlineAutoData("EmptySource.cs", 0)>]
    [<SyntaxTreeInlineAutoData("Class/SingleClass.cs", 1)>]
    [<SyntaxTreeInlineAutoData("Class/MultipleClass.cs", 2)>]
    [<SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", 2)>]
    [<SyntaxTreeInlineAutoData("Class/NestedClass.cs", 2)>]
    let ``Inspect_WhenUsingSourceFile_WithSuccess`` (count, tree : SyntaxTree) = 
        let classes = inspectClass (tree.GetRoot())

        Assert.Equal (count, classes.Length)
        Assert.True (query { for c in classes do all (c.Representation.Kind = StructureKind.Class) });