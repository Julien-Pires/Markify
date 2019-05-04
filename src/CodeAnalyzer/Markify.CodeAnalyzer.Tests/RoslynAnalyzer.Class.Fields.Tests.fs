namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassFields_Tests =
    [<Tests>]
    let noFieldsTests =
        let content = [
            (CSharp, ["
                public class WithoutFields {}
            "])
            (VisualBasic, ["
                Public Class WithoutFields
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no fields when class has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "WithoutFields"

                    test <@ result.Fields |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withFieldsTests =
        let content = [
            (CSharp, ["
                public class SingleField 
                {
                    Int32 FirstField;
                }
                public class MultipleField 
                {
                    Int32 FirstField;
                    Single SecondField;
                }
            "])
            (VisualBasic, ["
                Public Class SingleField
                    Dim FirstField As Int32
                End Class
                Public Class MultipleField
                    Dim FirstField As Int32
                    Dim SecondField As Single
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return fields when class has some" [
                (withProjects content, ("SingleField", 1))
                (withProjects content, ("MultipleField", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Fields |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct field name when class has some" [
                (withProjects content, ("SingleField", "FirstField"))
                (withProjects content, ("MultipleField", "SecondField"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Fields |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct field type when class has some" [
                (withProjects content, ("SingleField", "FirstField", "Int32"))
                (withProjects content, ("MultipleField", "SecondField", "Single"))]
                (fun sut project (name, field, expected) () ->
                    let result = sut.Analyze project |> findClass name

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = field)
                                          |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (CSharp, ["
                public class AccessModifier 
                {
                    Int32 WithoutAccessModifier;
                    public Int32 PublicField;
                    internal Int32 InternalField;
                    protected Int32 ProtectedField;
                    protected internal Int32 ProtectedInternalField;
                    private Int32 PrivateField;
                }
            "])
            (VisualBasic, ["
                Public Class AccessModifier
                    Dim WithoutAccessModifier As Int32
                    Public PublicField As Int32
                    Friend InternalField As Int32
                    Protected ProtectedField As Int32
                    Protected Friend ProtectedInternalField As Int32
                    Private PrivateField As Int32
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return correct access modifier for class field" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicField", Set ["public"]))
                (withProjects content, ("InternalField", Set ["internal"]))
                (withProjects content, ("ProtectedField", Set ["protected"]))
                (withProjects content, ("ProtectedInternalField", Set ["protected"; "internal"]))
                (withProjects content, ("PrivateField", Set ["private"]))]
                (fun sut project (field, expected) () ->
                    let info = sut.Analyze project |> findClass "AccessModifier"
                    let result = info.Fields |> Seq.find (fun c -> c.Name = field)

                    test <@ result.AccessModifiers |> Set
                                                   |> Set.map normalizeSyntax
                                                   |> (=) expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (CSharp, ["
                public class Modifier 
                {
                    Int32 WithoutModifier;
                    static Int32 StaticField;
                    readonly Int32 ReadonlyField;
                    static readonly Int32 StaticReadonlyField;
                    const Int32 ConstField;
                }
            "])
            (VisualBasic, ["
                Public Class Modifier
                    Private WithoutModifier As Int32
                    Shared StaticField As Int32
                    ReadOnly ReadonlyField As Int32
                    Shared ReadOnly StaticReadonlyField As Int32
                    Const ConstField As Int32
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no modifier when class field has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "Modifier"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "WithoutModifier")
                                          |> fun c -> c.Modifiers
                                          |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return modifier when class field has some" [
                (withProjects content, ("StaticField", Set ["static"]))
                (withProjects content, ("ReadonlyField", Set ["readonly"]))
                (withProjects content, ("StaticReadonlyField", Set ["static"; "readonly"]))
                (withProjects content, ("ConstField", Set ["const"]))]
                (fun sut project (field, expected) () ->
                    let info = sut.Analyze project |> findClass "Modifier"
                    let result = info.Fields |> Seq.find (fun c -> c.Name = field)

                    test <@ result.Modifiers |> Set
                                             |> Set.map normalizeSyntax
                                             |> (=) expected @>)
        ]

    [<Tests>]
    let defaultValueTests =
        let content = [
            (CSharp, ["
                public class DefaultValue 
                {
                    Int32 None;
                    Int32 Some = 1;
                }
            "])
            (VisualBasic, ["
                Public Class DefaultValue
                    Dim None As Int32
                    Dim Some As Int32 = 1
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no default value when class field has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "DefaultValue"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "None")
                                          |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeat (withProjects content)
                "should return default value when class field has one"
                (fun sut project () ->
                    let result = sut.Analyze project |> findClass "DefaultValue"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "Some")
                                          |> fun c -> c.DefaultValue = Some "1" @>)
        ]