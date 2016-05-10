module RoslynInspector_InspectClass
    open System
    open System.Linq
    open Microsoft.CodeAnalysis

    open Container
    open Inspectors
    open Representation

    open Xunit
    open Markify.Fixtures

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "abstract partial", "AbstractPartialClass")>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "sealed partial", "SealedPartialClass")>]
    let ``Inspect_WhenClassHasMultipleModifiers_WithCorrectModifiers`` (modifier: string, className, tree: SyntaxTree) =
        let inputModifiers = modifier.Split [|' '|]
        let testModifiers =
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.AdditionalModifiers)

        Assert.Equal (testModifiers.Count(), inputModifiers.Length)
        Assert.Equal (inputModifiers.Intersect(testModifiers).Count(), inputModifiers.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "SingleClass", 0)>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "InheritClass", 1)>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "ImplementInterfaceClass", 1)>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "ImplementGenInterfaceClass", 2)>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "MixedInheritanceClass", 3)>]
    let ``Inspect_WhenClassInherit_WithExactBaseType`` (className, count, tree: SyntaxTree) =
        let baseTypes = 
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.BaseTypes)

        Assert.Equal (count, baseTypes.Count())

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "InheritClass", "Exception")>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "ImplementInterfaceClass", "IDisposable")>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "ImplementGenInterfaceClass", "IList<String>")>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "MixedInheritanceClass", "Exception")>]
    [<SyntaxTreeInlineAutoData("Class/InheritedClass.cs", "MixedInheritanceClass", "IDisposable")>]
    let ``Inspect_WhenClassInherit_WithBaseTypeExist`` (className, baseType, tree: SyntaxTree) =
        let optBaseType = 
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.BaseTypes)
            |> Seq.tryFind (fun c -> c = baseType)

        Assert.True (optBaseType.IsSome);