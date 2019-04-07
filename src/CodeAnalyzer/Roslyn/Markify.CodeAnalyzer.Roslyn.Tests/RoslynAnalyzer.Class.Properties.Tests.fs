namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassProperties_Tests =
    [<Tests>]
    let noPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutProperties {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutProperties
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no properties when class has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "WithoutProperties"

                    test <@ result.Properties |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class SingleProperty
                {
                    Int32 Property { get; set; }
                }
                public class MultipleProperties
                {
                    public Int32 FirstProperty { get; set; }

                    public Int32 SecondProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class SingleProperty
                    Property Property() As Int32
                End Class
                Public Class MultipleProperties
                    Public Property FirstProperty() As Int32

                    Public Property SecondProperty() As Int32
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return properties when class has some" [
                (withProjects content, ("SingleProperty", 1))
                (withProjects content, ("MultipleProperties", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Properties |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct property name when class has some" [
                (withProjects content, ("SingleProperty", "Property"))
                (withProjects content, ("MultipleProperties", "SecondProperty"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name
                    
                    test <@ result.Properties |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeat (withProjects content)
                "should return correct class property type"
                (fun sut project () ->
                    let object = sut.Analyze project |> findClass "SingleProperty"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")
                    
                    test <@ result.Type = "Int32"  @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutAccessModifier
                {
                    Int32 PrivateProperty { get; set; }
                }
                public class WithAccessModifiers
                {
                    public Int32 AutoProperty { get; set; }
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                    protected Int32 ProtectedProperty { get; set; }
                    protected internal Int32 ProtectedInternalProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutAccessModifier
                    Property PrivateProperty() As Int32
                End Class
                Public Class WithAccessModifiers
                    Public Property AutoProperty() As Int32
                    Private Property PrivateProperty() As Int32
                    Protected Property ProtectedProperty() As Int32
                    Friend Property InternalProperty() As Int32
                    Protected Friend Property ProtectedInternalProperty() As Int32
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return private access modifier when class property has no specified access modifier"
                (fun sut project () ->
                    let object = sut.Analyze project |> findClass "WithoutAccessModifier"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "PrivateProperty")

                    test <@ result.AccessModifiers |> Seq.exists (fun c -> normalizeSyntax c = "private") @>)

            yield! testRepeatParameterized
                "should return correct access modifier when class property has some" [
                (withProjects content, ("AutoProperty", Set ["public"]))
                (withProjects content, ("InternalProperty", Set ["internal"]))
                (withProjects content, ("PrivateProperty", Set ["private"]))
                (withProjects content, ("ProtectedProperty", Set ["protected"]))
                (withProjects content, ("ProtectedInternalProperty", Set ["protected"; "internal"]))]
                (fun sut project (property, expected) () ->
                    let object = sut.Analyze project |> findClass "WithAccessModifiers"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                        
                    test <@ result.AccessModifiers |> Set 
                                                   |> Set.map normalizeSyntax 
                                                   |> (=) expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutModifiers
                {
                    public Int32 Property { get; set; }
                }
                public class WithModifiers
                {
                    public static Int32 StaticProperty { get; set; }
                    public virtual Int32 VirtualProperty { get; set; }
                    public sealed Int32 SealedProperty { get; set; }
                    public sealed override Int32 SealedOverrideProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutModifiers
                    Public Property Property() As Int32
                End Class
                Public Class WithModifiers
                    Public Shared Property StaticProperty() As Int32
                    Public Overridable Property VirtualProperty() As Int32
                    Public NotOverridable Property SealedProperty() As Int32
                    Public NotOverridable Overrides Property SealedOverrideProperty() As Int32
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return property with no modifiers when class property has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findClass "WithoutModifiers"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")

                    test <@ result.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return correct modifier when class property has some" [
                (withProjects content, ("StaticProperty", Set ["static"]))
                (withProjects content, ("VirtualProperty", Set ["virtual"]))
                (withProjects content, ("SealedProperty", Set ["sealed"]))
                (withProjects content, ("SealedOverrideProperty", Set ["sealed"; "override"]))]
                (fun sut project (property, expected) () ->
                    let object = sut.Analyze project |> findClass "WithModifiers"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                        
                    test <@ result.Modifiers |> Set 
                                             |> Set.map normalizeSyntax 
                                             |> (=) expected @>)
        ]

    [<Tests>]
    let defaultValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class DefaultValue
                {
                    public Int32 Without { get; set; }
                    public Int32 With { get; set; } = 1;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class DefaultValue
                    Public Property Without() As Int32
                    Public Property With() As Int32 = 1
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return property with no default value when class property has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "DefaultValue"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "Without") 
                                              |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeat (withProjects content)
                "should return property with default value when class property has one"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "DefaultValue"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "With") 
                                              |> fun c -> c.DefaultValue = Some "1" @>)
        ]

    [<Tests>]
    let writeAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutWriteAccessor
                {
                    public Int32 ReadOnly { get; }
                    public Int32 ReadOnlyWithDefaultValue { get; } = 1;
                }
                public class WithWriteAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Write { get; set; }
                    public Int32 WriteOnly { set {} }
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                    protected Int32 ProtectedProperty { get; set; }
                    protected internal Int32 ProtectedInternalProperty { get; set; }
                    Int32 PrivateWriteOnly { get; private set; }
                    Int32 InternalWriteOnly { get; internal set; }
                    Int32 ProtectedWriteOnly { get; protected set; }
                    Int32 ProtectedInternalWriteOnly { get; protected internal set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutWriteAccessor
                    Public ReadOnly Property ReadOnly() As Int32
                    Public ReadOnly Property ReadOnlyWithDefaultValue() As Int32 = 1
                End Class
                Public Class WithWriteAccessor
                    Property Auto() As Int32
                    Public Property Write() As Int32
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                    Private Property PrivateProperty() As Int32
                    Protected Property ProtectedProperty() As Int32
                    Friend Property InternalProperty() As Int32
                    Protected Friend Property ProtectedInternalProperty() As Int32
                    WriteOnly Property PrivateWriteOnly() As Int32
                        Private Set
                        End Set
                    End Property
                    WriteOnly Property InternalWriteOnly() As Int32
                        Friend Set
                        End Set
                    End Property
                    WriteOnly Property ProtectedWriteOnly() As Int32
                        Protected Set
                        End Set
                    End Property
                    WriteOnly Property ProtectedInternalWriteOnly() As Int32
                        Protected Friend Set
                        End Set
                    End Property
                End Class
            "])
        ]
        let cSharpContent = [
            (ProjectLanguage.CSharp, ["
                public class WithoutWriteAccessor
                {
                    public Int32 ReadOnlyWithExpressionBody => 1;
                }
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return no write accessor when class property is read-only" [
                (withProjects content, "ReadOnly")
                (withProjects content, "ReadOnlyWithDefaultValue")
                (withProjects cSharpContent, "ReadOnlyWithExpressionBody")]
                (fun sut project property () ->
                    let result = sut.Analyze project |> findClass "WithoutWriteAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsNone @>)

            yield! testRepeatParameterized
                "should return write accessor when class property has one" [
                (withProjects content, "Write")
                (withProjects content, "WriteOnly")]
                (fun sut project property () ->
                    let result = sut.Analyze project |> findClass "WithWriteAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsSome @>)

            yield! testRepeatParameterized
                "should return correct write accessor access modifier when class property has some"[
                (withProjects content, ("Auto", Set ["private"]))
                (withProjects content, ("Write", Set ["public"]))
                (withProjects content, ("PrivateProperty", Set ["private"]))
                (withProjects content, ("PrivateWriteOnly", Set ["private"]))
                (withProjects content, ("InternalProperty", Set ["internal"]))
                (withProjects content, ("InternalWriteOnly", Set ["internal"]))
                (withProjects content, ("ProtectedProperty", Set ["protected"]))
                (withProjects content, ("ProtectedWriteOnly", Set ["protected"]))
                (withProjects content, ("ProtectedInternalProperty", Set ["protected"; "internal"]))
                (withProjects content, ("ProtectedInternalWriteOnly", Set ["protected"; "internal"]))]
                (fun sut project (property, expected) () ->
                    let object = sut.Analyze project |> findClass "WithWriteAccessor"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsWrite.Value.AccessModifiers |> Set
                                                                 |> Set.map normalizeSyntax 
                                                                 |> (=) expected @>)
        ]
    
    [<Tests>]
    let readAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutReadAccessor
                {
                    public Int32 WriteOnly { set {} }
                }
                public class WithReadAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Read { get; set; }
                    public Int32 ReadOnly { get; }
                    public Int32 ReadOnlyWithDefaultValue { get; } = 1;
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                    protected Int32 ProtectedProperty { get; set; }
                    protected internal Int32 ProtectedInternalProperty { get; set; }
                    Int32 PrivateReadOnly { private get; set; }
                    Int32 InternalReadOnly { internal get; set; }
                    Int32 ProtectedReadOnly { protected get; set; }
                    Int32 ProtectedInternalReadOnly { protected internal get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutReadAccessor
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                End Class
                Public Class WithReadAccessor
                    Property Auto() As Int32
                    Public Property Read() As Int32
                    Public ReadOnly Property ReadOnly() As Int32
                    Public ReadOnly Property ReadOnlyWithDefaultValue() As Int32 = 1
                    Private Property PrivateProperty() As Int32
                    Protected Property ProtectedProperty() As Int32
                    Friend Property InternalProperty() As Int32
                    Protected Friend Property ProtectedInternalProperty() As Int32
                    ReadOnly Property PrivateReadOnly() As Int32
                        Private Get
                        End Get
                    End Property
                    ReadOnly Property InternalReadOnly() As Int32
                        Friend Get
                        End Get
                    End Property
                    ReadOnly Property ProtectedReadOnly() As Int32
                        Protected Get
                        End Get
                    End Property
                    ReadOnly Property ProtectedInternalReadOnly() As Int32
                        Protected Friend Get
                        End Get
                    End Property
                End Class
            "])
        ]
        let cSharpContent = [
            (ProjectLanguage.CSharp, ["
                public class WithReadAccessor
                {
                    public Int32 ReadOnlyWithExpressionBody => 1;
                }
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no read accessor when class property is write-only"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "WithoutReadAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "WriteOnly")
                                              |> fun c -> c.IsRead.IsNone @>)

            yield! testRepeatParameterized
                "should return read accessor when class property has one" [
                (withProjects content, "Auto")
                (withProjects content, "ReadOnly")
                (withProjects content, "ReadOnlyWithDefaultValue")
                (withProjects cSharpContent, "ReadOnlyWithExpressionBody")]
                (fun sut project property () ->
                    let result = sut.Analyze project |> findClass "WithReadAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsRead.IsSome @>)

            yield! testRepeatParameterized
                "should return correct read accessor access modifier when class property has some"[
                (withProjects content, ("Auto", Set ["private"]))
                (withProjects content, ("Read", Set ["public"]))
                (withProjects content, ("PrivateProperty", Set ["private"]))
                (withProjects content, ("PrivateReadOnly", Set ["private"]))
                (withProjects content, ("InternalProperty", Set ["internal"]))
                (withProjects content, ("InternalReadOnly", Set ["internal"]))
                (withProjects content, ("ProtectedProperty", Set ["protected"]))
                (withProjects content, ("ProtectedReadOnly", Set ["protected"]))
                (withProjects content, ("ProtectedInternalProperty", Set ["protected"; "internal"]))
                (withProjects content, ("ProtectedInternalReadOnly", Set ["protected"; "internal"]))]
                (fun sut project (property, expected) () ->
                    let object = sut.Analyze project |> findClass "WithReadAccessor"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsRead.Value.AccessModifiers |> Set
                                                                |> Set.map normalizeSyntax
                                                                |> (=) expected @>)
        ]