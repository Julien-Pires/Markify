namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceProperties_Tests =
    [<Tests>]
    let noPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutProperties {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutProperties
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no properties when interface has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "WithoutProperties"

                    test <@ result.Properties |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withPropertiesTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface SingleProperty
                {
                    Int32 Property { get; set; }
                }
                public interface MultipleProperties
                {
                    public Int32 FirstProperty { get; set; }

                    public Int32 SecondProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface SingleProperty
                    Property Property() As Int32
                End Interface
                Public Interface MultipleProperties
                    Public Property FirstProperty() As Int32

                    Public Property SecondProperty() As Int32
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return properties when interface has some" [
                (withProjects content, ("SingleProperty", 1))
                (withProjects content, ("MultipleProperties", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name

                    test <@ result.Properties |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct property name when interface has some" [
                (withProjects content, ("SingleProperty", "Property"))
                (withProjects content, ("MultipleProperties", "SecondProperty"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                    
                    test <@ result.Properties |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeat (withProjects content)
                "should return correct interface property type"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies "SingleProperty"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")
                    
                    test <@ result.Type = "Int32"  @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutAccessModifier
                {
                    Int32 PublicProperty { get; set; }
                }
                public interface WithAccessModifiers
                {
                    public Int32 PublicProperty { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutAccessModifier
                    Property PublicProperty() As Int32
                End Interface
                Public Interface WithAccessModifiers
                    Public Property PublicProperty() As Int32
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return public access modifier when interface property has no specified access modifier"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies "WithoutAccessModifier"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "PublicProperty")

                    test <@ result.AccessModifiers |> Seq.exists (fun c -> normalizeSyntax c = "public") @>)

            yield! testRepeatParameterized
                "should return correct access modifier when interface property has some" [
                (withProjects content, ("WithAccessModifiers", "PublicProperty", Set ["public"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                        
                    test <@ result.AccessModifiers |> Seq.map normalizeSyntax 
                                                   |> fun c -> Set c |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutModifiers
                {
                    public Int32 Property { get; set; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutModifiers
                    Public Property Property() As Int32
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return property with no modifiers when interface property has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies "WithoutModifiers"
                    let result = object.Properties |> Seq.find (fun c -> c.Name = "Property")

                    test <@ result.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let writeAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutWriteAccessor
                {
                    public Int32 ReadOnly { get; }
                }
                public interface WithWriteAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Write { get; set; }
                    public Int32 WriteOnly { set {} }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutWriteAccessor
                    Public ReadOnly Property ReadOnly() As Int32
                End Interface
                Public Interface WithWriteAccessor
                    Property Auto() As Int32
                    Public Property Write() As Int32
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return no write accessor when interface property is read-only" [
                (withProjects content, ("WithoutWriteAccessor", "ReadOnly"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsNone @>)

            yield! testRepeatParameterized
                "should return write accessor when interface property has one" [
                (withProjects content, ("WithWriteAccessor", "Write"))
                (withProjects content, ("WithWriteAccessor", "WriteOnly"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsWrite.IsSome @>)

            yield! testRepeatParameterized
                "should return correct write accessor access modifier when interface property has some"[
                (withProjects content, ("WithWriteAccessor", "Auto", Set ["public"]))
                (withProjects content, ("WithWriteAccessor", "Write", Set ["public"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsWrite.Value.AccessModifiers |> Seq.map normalizeSyntax
                                                                 |> fun c -> Set c |> Set.isSubset expected @>)
        ]
    
    [<Tests>]
    let readAccessorTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutReadAccessor
                {
                    public Int32 WriteOnly { set {} }
                }
                public interface WithReadAccessor
                {
                    Int32 Auto { get; set; }
                    public Int32 Read { get; set; }
                    public Int32 ReadOnly { get; }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutReadAccessor
                    Public WriteOnly Property WriteOnly() As Int32
                        Set
                        End Set
                    End Property
                End Interface
                Public Interface WithReadAccessor
                    Property Auto() As Int32
                    Public Property Read() As Int32
                    Public ReadOnly Property ReadOnly() As Int32
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no read accessor when interface property is write-only"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "WithoutReadAccessor"
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = "WriteOnly")
                                              |> fun c -> c.IsRead.IsNone @>)

            yield! testRepeatParameterized
                "should return read accessor when interface property has one" [
                (withProjects content, ("WithReadAccessor", "Auto"))
                (withProjects content, ("WithReadAccessor", "ReadOnly"))]
                (fun sut project (name, property) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                    
                    test <@ result.Properties |> Seq.find (fun c -> c.Name = property)
                                              |> fun c -> c.IsRead.IsSome @>)

            yield! testRepeatParameterized
                "should return correct read accessor access modifier when interface property has some"[
                (withProjects content, ("WithReadAccessor", "Auto", Set ["public"]))
                (withProjects content, ("WithReadAccessor", "Read", Set ["public"]))]
                (fun sut project (name, property, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findInterface assemblies name
                    let result = object.Properties |> Seq.find (fun c -> c.Name = property)
                    
                    test <@ result.IsRead.Value.AccessModifiers |> Seq.map normalizeSyntax
                                                                |> fun c -> Set c |> Set.isSubset expected @>)
        ]