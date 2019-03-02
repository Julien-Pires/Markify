namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructFields_Tests =
    [<Tests>]
    let noFieldsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutFields {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutFields
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no fields when struct has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "WithoutFields"

                    test <@ result.Fields |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withFieldsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct SingleField 
                {
                    Int32 FirstField;
                }
                public struct MultipleField 
                {
                    Int32 FirstField;
                    Single SecondField;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleField
                    Dim FirstField As Int32
                End Structure
                Public Structure MultipleField
                    Dim FirstField As Int32
                    Dim SecondField As Single
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return fields when struct has some" [
                (withProjects content, ("SingleField", 1))
                (withProjects content, ("MultipleField", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Fields |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct field name when struct has some" [
                (withProjects content, ("SingleField", "FirstField"))
                (withProjects content, ("MultipleField", "SecondField"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Fields |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct field type when struct has some" [
                (withProjects content, ("SingleField", "FirstField", "Int32"))
                (withProjects content, ("MultipleField", "SecondField", "Single"))]
                (fun sut project (name, field, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = field)
                                          |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let accessModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct AccessModifier 
                {
                    Int32 WithoutAccessModifier;
                    public Int32 PublicField;
                    internal Int32 InternalField;
                    private Int32 PrivateField;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure AccessModifier
                    Dim WithoutAccessModifier As Int32
                    Public PublicField As Int32
                    Friend InternalField As Int32
                    Private PrivateField As Int32
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return correct access modifier for struct field" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicField", Set ["public"]))
                (withProjects content, ("InternalField", Set ["internal"]))
                (withProjects content, ("PrivateField", Set ["private"]))]
                (fun sut project (field, expected) () ->
                    let object = sut.Analyze project |> findStruct "AccessModifier"
                    let result = object.Fields |> Seq.find (fun c -> c.Name = field)

                    test <@ result.AccessModifiers |> Seq.map normalizeSyntax
                                                   |> Set
                                                   |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct Modifier 
                {
                    Int32 WithoutModifier;
                    static Int32 StaticField;
                    readonly Int32 ReadonlyField;
                    static readonly Int32 StaticReadonlyField;
                    const Int32 ConstField;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure Modifier
                    Private WithoutModifier As Int32
                    Shared StaticField As Int32
                    ReadOnly ReadonlyField As Int32
                    Shared ReadOnly StaticReadonlyField As Int32
                    Const ConstField As Int32
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no modifier when struct field has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "Modifier"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "WithoutModifier")
                                          |> fun c -> c.Modifiers
                                          |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return modifier when struct field has some" [
                (withProjects content, ("StaticField", Set ["static"]))
                (withProjects content, ("ReadonlyField", Set ["readonly"]))
                (withProjects content, ("StaticReadonlyField", Set ["static"; "readonly"]))
                (withProjects content, ("ConstField", Set ["const"]))]
                (fun sut project (field, expected) () ->
                    let object = sut.Analyze project |> findStruct "Modifier"
                    let result = object.Fields |> Seq.find (fun c -> c.Name = field)

                    test <@ result.Modifiers |> Seq.map normalizeSyntax
                                             |> Set
                                             |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let defaultValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct DefaultValue 
                {
                    Int32 None;
                    Int32 Some = 1;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure DefaultValue
                    Dim None As Int32
                    Dim Some As Int32 = 1
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no default value when struct field has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "DefaultValue"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "None")
                                          |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeat (withProjects content)
                "should return default value when struct field has one"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "DefaultValue"

                    test <@ result.Fields |> Seq.find (fun c -> c.Name = "Some")
                                          |> fun c -> c.DefaultValue = Some "1" @>)
        ]