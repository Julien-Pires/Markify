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
    [<SyntaxTreeInlineAutoData("EmptySource.cs", 0)>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", 4)>]
    let ``Inspect_WhenVariousContexts_WithSuccess`` (count, tree: SyntaxTree) = 
        let classes = inspectClass (tree.GetRoot())

        Assert.Equal (count, classes.Length)
        Assert.True (query { for c in classes do all (c.Representation.Kind = StructureKind.Class) })
    
    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AccessModifier.cs", "public", "PublicClass")>]
    [<SyntaxTreeInlineAutoData("Class/AccessModifier.cs", "internal", "InternalClass")>]
    [<SyntaxTreeInlineAutoData("Class/AccessModifier.cs", "protected", "ProtectedClass")>]
    [<SyntaxTreeInlineAutoData("Class/AccessModifier.cs", "protected internal", "ProtectedInternalClass")>]
    [<SyntaxTreeInlineAutoData("Class/AccessModifier.cs", "private", "PrivateClass")>]
    let ``Inspect_WhenClassHasAccessModifier_WithCorrectAccessModifier`` (modifier, className, tree: SyntaxTree) =
        let testClass = 
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)

        Assert.Equal (modifier, String.Join (" ", testClass.Representation.AccessModifiers))

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "abstract", "AbstractClass")>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "sealed", "SealedClass")>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "partial", "PartialClass")>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "static", "StaticClass")>]
    let ``Inspect_WhenClassHasSingleModifier_WithCorrectModifier`` (modifier, className, tree: SyntaxTree) =
        let classModifier =
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.AdditionalModifiers)
            |> (fun c -> List.item 0 c)

        Assert.Equal (classModifier, modifier)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "abstract partial", "AbstractPartialClass")>]
    [<SyntaxTreeInlineAutoData("Class/AdditionnalModifier.cs", "sealed partial", "SealedPartialClass")>]
    let ``Inspect_WhenClassHasMultipleModifiers_WithCorrectModifiers`` (modifier: string, className, tree: SyntaxTree) =
        let inputModifiers = modifier.Split [|' '|]
        let testModifiers =
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.AdditionalModifiers)

        Assert.Equal (testModifiers.Length, inputModifiers.Length)
        Assert.Equal (inputModifiers.Intersect(testModifiers).Count(), inputModifiers.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", 0, "SingleClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2, "GenericClass")>]
    let ``Inspect_WhenClassIsGeneric_WithAllParameters`` (count, className, tree: SyntaxTree) =
        let parameters =
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)
            |> (fun c -> c.Representation.GenericParameters)

        Assert.Equal (count, parameters.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "ParentClass")>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "InNamespaceClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "GenericClass'2")>]
    let ``Inspect_WithCorrectName`` (className, tree: SyntaxTree) =
        let testClass = 
            inspectClass (tree.GetRoot())
            |> Seq.tryFind (fun c -> Name c.Representation = className)

        Assert.True (testClass.IsSome)
    
    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "SingleClass", "SingleClass")>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "NestedClass","ParentClass.NestedClass")>]
    [<SyntaxTreeInlineAutoData("Class/ClassSamples.cs", "InNamespaceClass","FooSpace.InNamespaceClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "GenericClass'2","GenericClass'2")>]
    let ``Inspect_WithCorrectFullname`` (className, fullname, tree: SyntaxTree) =
        let testClass = 
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)

        Assert.Equal (fullname, String.Join (".", Fullname testClass))

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

        Assert.Equal (count, baseTypes.Length)

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