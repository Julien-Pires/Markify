namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Services.Roslyn
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructProperties_Tests =
    [<Tests>]
    let noPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutProperties {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutProperties
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no properties when struct has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "WithoutProperties"

                    test <@ result.Properties |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct SingleProperty
                {
                    Int32 Property { get; set; }
                }
                public struct MultipleProperties
                {
                    public Int32 FirstProperty { get; set; }

                    public Int32 SecondProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleProperty
                    Property Property() As Int32
                End Structure
                Public Structure MultipleProperties
                    Public Property FirstProperty() As Int32

                    Public Property SecondProperty() As Int32
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return properties when struct has some" [
                (withProjects content, ("SingleProperty", 1))
                (withProjects content, ("MultipleProperties", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name

                    test <@ result.Properties |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct property name when struct has some" [
                (withProjects content, ("SingleProperty", "Property"))
                (withProjects content, ("MultipleProperties", "SecondProperty"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                    
                    test <@ result.Properties |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeat (withProjects content)
                "should return correct struct property type"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies "SingleProperty"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")
                    
                    test <@ result.Type = "Int32"  @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutAccessModifier
                {
                    Int32 PrivateProperty { get; set; }
                }
                public struct WithAccessModifiers
                {
                    public Int32 AutoProperty { get; set; }
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutAccessModifier
                    Property PrivateProperty() As Int32
                End Structure
                Public Structure WithAccessModifiers
                    Public Property AutoProperty() As Int32
                    Private Property PrivateProperty() As Int32
                    Friend Property InternalProperty() As Int32
                End Structure
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return private access modifier when struct property has no specified access modifier"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies "WithoutAccessModifier"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "PrivateProperty")

                    test <@ result.AccessModifiers |> Seq.exists (fun c -> normalizeSyntax c = "private") @>)

            yield! testRepeatParameterized
                "should return correct access modifier when struct property has some" [
                (withProjects content, ("WithAccessModifiers", "AutoProperty", Set ["public"]))
                (withProjects content, ("WithAccessModifiers", "InternalProperty", Set ["internal"]))
                (withProjects content, ("WithAccessModifiers", "PrivateProperty", Set ["private"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                        
                    test <@ result.AccessModifiers |> Seq.map normalizeSyntax 
                                                   |> fun c -> Set c |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutModifiers
                {
                    public Int32 Property { get; set; }
                }
                public struct WithModifiers
                {
                    public static Int32 StaticProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutModifiers
                    Public Property Property() As Int32
                End Structure
                Public Structure WithModifiers
                    Public Shared Property StaticProperty() As Int32
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return property with no modifiers when struct property has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies "WithoutModifiers"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")

                    test <@ result.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return correct modifier when struct property has some" [
                (withProjects content, ("WithModifiers", "StaticProperty", Set ["static"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                        
                    test <@ result.Modifiers |> Seq.map normalizeSyntax
                                             |> fun c -> Set c |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let defaultValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct DefaultValue
                {
                    public Int32 Without { get; set; }
                    public Int32 With { get; set; } = 1;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure DefaultValue
                    Public Property Without() As Int32
                    Public Property With() As Int32 = 1
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return property with no default value when struct property has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "DefaultValue"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "Without") 
                                              |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeat (withProjects content)
                "should return property with default value when struct property has one"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "DefaultValue"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "With") 
                                              |> fun c -> c.DefaultValue = Some "1" @>)
        ]

    [<Tests>]
    let writeAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutWriteAccessor
                {
                    public Int32 ReadOnly { get; }
                    public Int32 ReadOnlyWithDefaultValue { get; } = 1;
                }
                public struct WithWriteAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Write { get; set; }
                    public Int32 WriteOnly { set {} }
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                    Int32 PrivateWriteOnly { get; private set; }
                    Int32 InternalWriteOnly { get; internal set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutWriteAccessor
                    Public ReadOnly Property ReadOnly() As Int32
                    Public ReadOnly Property ReadOnlyWithDefaultValue() As Int32 = 1
                End Structure
                Public Structure WithWriteAccessor
                    Property Auto() As Int32
                    Public Property Write() As Int32
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                    Private Property PrivateProperty() As Int32
                    Friend Property InternalProperty() As Int32
                    WriteOnly Property PrivateWriteOnly() As Int32
                        Private Set
                        End Set
                    End Property
                    WriteOnly Property InternalWriteOnly() As Int32
                        Friend Set
                        End Set
                    End Property
                End Structure
            "])
        ]
        let cSharpContent = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutWriteAccessor
                {
                    public Int32 ReadOnlyWithExpressionBody => 1;
                }
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return no write accessor when struct property is read-only" [
                (withProjects content, ("WithoutWriteAccessor", "ReadOnly"))
                (withProjects content, ("WithoutWriteAccessor", "ReadOnlyWithDefaultValue"))
                (withProjects cSharpContent, ("WithoutWriteAccessor", "ReadOnlyWithExpressionBody"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsNone @>)

            yield! testRepeatParameterized
                "should return write accessor when struct property has one" [
                (withProjects content, ("WithWriteAccessor", "Write"))
                (withProjects content, ("WithWriteAccessor", "WriteOnly"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsSome @>)

            yield! testRepeatParameterized
                "should return correct write accessor access modifier when struct property has some"[
                (withProjects content, ("WithWriteAccessor", "Auto", Set ["private"]))
                (withProjects content, ("WithWriteAccessor", "Write", Set ["public"]))
                (withProjects content, ("WithWriteAccessor", "PrivateProperty", Set ["private"]))
                (withProjects content, ("WithWriteAccessor", "PrivateWriteOnly", Set ["private"]))
                (withProjects content, ("WithWriteAccessor", "InternalProperty", Set ["internal"]))
                (withProjects content, ("WithWriteAccessor", "InternalWriteOnly", Set ["internal"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsWrite.Value.AccessModifiers |> Seq.map normalizeSyntax
                                                                 |> fun c -> Set c |> Set.isSubset expected @>)
        ]
    
    [<Tests>]
    let readAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutReadAccessor
                {
                    public Int32 WriteOnly { set {} }
                }
                public struct WithReadAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Read { get; set; }
                    public Int32 ReadOnly { get; }
                    public Int32 ReadOnlyWithDefaultValue { get; } = 1;
                    private Int32 PrivateProperty { get; set; }
                    internal Int32 InternalProperty { get; set; }
                    Int32 PrivateReadOnly { private get; set; }
                    Int32 InternalReadOnly { internal get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutReadAccessor
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                End Structure
                Public Structure WithReadAccessor
                    Property Auto() As Int32
                    Public Property Read() As Int32
                    Public ReadOnly Property ReadOnly() As Int32
                    Public ReadOnly Property ReadOnlyWithDefaultValue() As Int32 = 1
                    Private Property PrivateProperty() As Int32
                    Friend Property InternalProperty() As Int32
                    ReadOnly Property PrivateReadOnly() As Int32
                        Private Get
                        End Get
                    End Property
                    ReadOnly Property InternalReadOnly() As Int32
                        Friend Get
                        End Get
                    End Property
                End Structure
            "])
        ]
        let cSharpContent = [
            (ProjectLanguage.CSharp, ["
                public struct WithReadAccessor
                {
                    public Int32 ReadOnlyWithExpressionBody => 1;
                }
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no read accessor when struct property is write-only"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "WithoutReadAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "WriteOnly")
                                              |> fun c -> c.IsRead.IsNone @>)

            yield! testRepeatParameterized
                "should return read accessor when struct property has one" [
                (withProjects content, ("WithReadAccessor", "Auto"))
                (withProjects content, ("WithReadAccessor", "ReadOnly"))
                (withProjects content, ("WithReadAccessor", "ReadOnlyWithDefaultValue"))
                (withProjects cSharpContent, ("WithReadAccessor", "ReadOnlyWithExpressionBody"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsRead.IsSome @>)

            yield! testRepeatParameterized
                "should return correct read accessor access modifier when struct property has some"[
                (withProjects content, ("WithReadAccessor", "Auto", Set ["private"]))
                (withProjects content, ("WithReadAccessor", "Read", Set ["public"]))
                (withProjects content, ("WithReadAccessor", "PrivateProperty", Set ["private"]))
                (withProjects content, ("WithReadAccessor", "PrivateReadOnly", Set ["private"]))
                (withProjects content, ("WithReadAccessor", "InternalProperty", Set ["internal"]))
                (withProjects content, ("WithReadAccessor", "InternalReadOnly", Set ["internal"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findStruct assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsRead.Value.AccessModifiers |> Seq.map normalizeSyntax
                                                                |> fun c -> Set c |> Set.isSubset expected @>)
        ]