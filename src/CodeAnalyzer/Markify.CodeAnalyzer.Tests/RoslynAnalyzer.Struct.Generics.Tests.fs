namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructGenerics_Tests =
    [<Tests>]
    let noGenericStructTests =
        let noGeneric = [
            (CSharp, ["
                public struct NoGenericStruct { }
            "])
            (VisualBasic, ["
                Public Structure NoGenericStruct
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects noGeneric)
                "should return no generic parameters when struct has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoGenericStruct"

                    test <@ result.Generics |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericStructTests =
        let generic = [
            (CSharp, ["
                public struct SingleGenericStruct<T> { }
                public struct MultipleGenericStruct<T, Y> { }
            "])
            (VisualBasic, ["
                Public Structure SingleGenericStruct(Of T)
                End Structure
                Public Structure MultipleGenericStruct(Of T, Y)
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized 
                "should return generic parameters when struct has some" [
                (withProjects generic, ("SingleGenericStruct`1", 1))
                (withProjects generic, ("MultipleGenericStruct`2", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name 
                        
                    test <@ result.Generics |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when struct has some" [
                (withProjects generic, ("SingleGenericStruct`1", "T"))
                (withProjects generic, ("MultipleGenericStruct`2", "T"))
                (withProjects generic, ("MultipleGenericStruct`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (CSharp, ["
                public struct SingleGenericStruct<T> { }
            "])
            (VisualBasic, ["
                Public Structure SingleGenericStruct(Of T)
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects genericModifiers) 
                "should return no modifiers when struct generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericStruct`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Modifier.IsNone @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (CSharp, ["
                public struct SingleGenericStruct<T> { }
                public struct GenericConstrainedStruct<T, Y>
                    where T : struct
                    where Y : IEnumerable, class, new() { }
            "])
            (VisualBasic, ["
                Public Structure SingleGenericStruct(Of T)
                End Structure
                Public Structure GenericConstrainedStruct(Of T As Structure, Y As { IEnumerable, Class, New })
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects genericConstraints)
                "should return no constraints when struct generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericStruct`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when struct generic parameter has some" [
                (withProjects genericConstraints, ("T", Set ["struct"]))
                (withProjects genericConstraints, ("Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (parameter, expected) () ->
                    let definition = sut.Analyze project |> findType "GenericConstrainedStruct`2"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Set
                                               |> Set.map normalizeSyntax
                                               |> (=) expected @>)
        ]