﻿namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructGenerics_Tests =
    [<Tests>]
    let noGenericStructTests =
        let noGeneric = [
            (ProjectLanguage.CSharp, ["
                public struct NoGenericStruct { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure NoGenericStruct
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized 
                "should return no generic parameters when struct has none"
                [(withProjects noGeneric, "NoGenericStruct")]
                (fun sut project name () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericStructTests =
        let generic = [
            (ProjectLanguage.CSharp, ["
                public struct SingleGenericStruct<T> { }
                public struct MultipleGenericStruct<T, Y> { }
            "])
            (ProjectLanguage.VisualBasic, ["
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
                    let result = sut.Analyze project |> findStruct name 
                        
                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when struct has some" [
                (withProjects generic, ("SingleGenericStruct`1", "T"))
                (withProjects generic, ("MultipleGenericStruct`2", "T"))
                (withProjects generic, ("MultipleGenericStruct`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name
                        
                    test <@ result.Identity.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (ProjectLanguage.CSharp, ["
                public struct SingleGenericStruct<T> { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleGenericStruct(Of T)
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized 
                "should return no modifiers when struct generic parameter has none" [
                (withProjects genericModifiers, ("SingleGenericStruct`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let object = sut.Analyze project |> findStruct name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Modifier.IsNone @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (ProjectLanguage.CSharp, ["
                public struct SingleGenericStruct<T> { }
                public struct GenericConstrainedStruct<T, Y>
                    where T : struct
                    where Y : IEnumerable, class, new() { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleGenericStruct(Of T)
                End Structure
                Public Structure GenericConstrainedStruct(Of T As Structure, Y As { IEnumerable, Class, New })
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized 
                "should return no constraints when struct generic parameter has none" [
                (withProjects genericConstraints, ("SingleGenericStruct`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let object = sut.Analyze project |> findStruct name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when struct generic parameter has some" [
                (withProjects genericConstraints, ("GenericConstrainedStruct`2", "T", Set ["struct"]))
                (withProjects genericConstraints, ("GenericConstrainedStruct`2", "Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (name, parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Seq.map normalizeSyntax
                                               |> Set
                                               |> Set.isSubset expected @>)
        ]