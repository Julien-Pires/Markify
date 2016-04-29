module RoslynInspector
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
    [<SyntaxTreeInlineAutoData("Class/SingleClass.cs", 1)>]
    [<SyntaxTreeInlineAutoData("Class/MultipleClass.cs", 2)>]
    [<SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", 2)>]
    [<SyntaxTreeInlineAutoData("Class/NestedClass.cs", 2)>]
    let ``Inspect_WhenVariousContexts_WithSuccess`` (count, tree: SyntaxTree) = 
        let classes = inspectClass (tree.GetRoot())

        Assert.Equal (count, classes.Length)
        Assert.True (query { for c in classes do all (c.Representation.Kind = StructureKind.Class) })
    
    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/SingleClass.cs", "public", "SingleClassWithoutNamespace")>]
    [<SyntaxTreeInlineAutoData("Class/InternalClass.cs", "internal", "InternalClass")>]
    [<SyntaxTreeInlineAutoData("Class/ProtectedClass.cs", "protected", "ProtectedClass")>]
    [<SyntaxTreeInlineAutoData("Class/ProtectedInternalClass.cs", "protected internal", "ProtectedInternalClass")>]
    [<SyntaxTreeInlineAutoData("Class/PrivateClass.cs", "private", "PrivateClass")>]
    let ``Inspect_WhenClassHasAccessModifier_WithCorrectAccessModifier`` (modifier, className, tree: SyntaxTree) =
        let testClass = 
            inspectClass (tree.GetRoot())
            |> Seq.find (fun c -> Name c.Representation = className)

        Assert.Equal (modifier, String.Join (" ", testClass.Representation.AccessModifiers))

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "abstract")>]
    [<SyntaxTreeInlineAutoData("Class/SealedClass.cs", "sealed")>]
    [<SyntaxTreeInlineAutoData("Class/PartialClass.cs", "partial")>]
    [<SyntaxTreeInlineAutoData("Class/StaticClass.cs", "static")>]
    let ``Inspect_WhenClassHasSingleModifier_WithCorrectModifier`` (modifier, tree: SyntaxTree) =
        let classModifier =
            inspectClass (tree.GetRoot())
            |> List.item 0
            |> (fun c -> c.Representation.AdditionalModifiers)
            |> (fun c -> List.item 0 c)

        Assert.Equal (classModifier, modifier)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AbstractPartialClass.cs", "abstract partial")>]
    [<SyntaxTreeInlineAutoData("Class/SealedPartialClass.cs", "sealed partial")>]
    let ``Inspect_WhenClassHasMultipleModifiers_WithCorrectModifiers`` (modifier: string, tree: SyntaxTree) =
        let inputModifiers = modifier.Split [|' '|]
        let testModifiers =
            inspectClass (tree.GetRoot())
            |> List.item 0
            |> (fun c -> c.Representation.AdditionalModifiers)

        Assert.Equal (testModifiers.Length, inputModifiers.Length)
        Assert.Equal (inputModifiers.Intersect(testModifiers).Count(), inputModifiers.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/SingleClass.cs", 0)>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2)>]
    let ``Inspect_WhenClassIsGeneric_WithAllParameters`` (count, tree: SyntaxTree) =
        let parameters =
            inspectClass (tree.GetRoot())
            |> List.item 0
            |> (fun c -> c.Representation.GenericParameters)

        Assert.Equal (count, parameters.Length)

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "AbstractClass")>]
    [<SyntaxTreeInlineAutoData("Class/InternalClass.cs", "InternalClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "GenericClass'2")>]
    let ``Inspect_WithCorrectName`` (name, tree: SyntaxTree) =
        let className = 
            inspectClass (tree.GetRoot())
            |> List.item 0
            |> (fun c -> Name c.Representation)

        Assert.Equal (name, className)
    
    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "AbstractClass")>]
    [<SyntaxTreeInlineAutoData("Class/ProtectedClass.cs", "ProtectedFoo.ProtectedClass")>]
    [<SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", "FooSpace.InsideNamespaceClass")>]
    [<SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "GenericClass'2")>]
    let ``Inspect_WithCorrectFullname`` (fullname: string, tree: SyntaxTree) =
        let classFullname = 
            inspectClass (tree.GetRoot())
            |> List.item 0
            |> (fun c -> Fullname c)

        Assert.Equal (fullname, String.Join (".", classFullname))

    [<Theory>]
    [<SyntaxTreeInlineAutoData("Class/SingleClass.cs", "SingleClassWithoutNamespace", 0)>]
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