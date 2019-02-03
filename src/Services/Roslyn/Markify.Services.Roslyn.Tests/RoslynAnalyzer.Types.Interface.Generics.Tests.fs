﻿namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesInterfaceGenericsTests =
    [<Tests>]
    let noGenericInterfaceTests =
        let noGeneric = [
            (
                ProjectLanguage.CSharp,
                ["
                    public interface NoGenericInterface { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Interface NoGenericInterface
                    End Interface
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no generic parameters when interface has none"
                [(withProjects noGeneric, "NoGenericInterface")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericInterfaceTests =
        let generic = [
            (
                ProjectLanguage.CSharp,
                ["
                    public interface SingleGenericInterface<T> { }

                    public interface MultipleGenericInterface<T, Y> { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Interface SingleGenericInterface(Of T)
                    End Interface

                    Public Interface MultipleGenericInterface(Of T, Y)
                    End Interface
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return generic parameters when interface has some" [
                (withProjects generic, ("SingleGenericInterface`1", 1))
                (withProjects generic, ("MultipleGenericInterface`2", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name 
                        
                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized "should return valid generic parameter name when struct has some" [
                (withProjects generic, ("SingleGenericInterface`1", "T"))
                (withProjects generic, ("MultipleGenericInterface`2", "T"))
                (withProjects generic, ("MultipleGenericInterface`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                        
                    test <@ result.Identity.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (
                ProjectLanguage.CSharp,
                ["
                    public interface SingleGenericInterface<T> { }

                    public interface CovariantGenericInterface<in T> { }

                    public interface ContravariantGenericInterface<out T> { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Interface SingleGenericInterface(Of T)
                    End Interface

                    Public Interface CovariantGenericInterface(Of In T)
                    End Interface

                    Public Interface ContravariantGenericInterface(Of Out T)
                    End Interface
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no modifiers when interface generic parameter has none"
                [withProjects genericModifiers, ("SingleGenericInterface`1", "T")]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Modifier.IsNone @>)

            yield! testRepeatParameterized "should return modifier when interface generic parameter has one"  [
                (withProjects genericModifiers, ("CovariantGenericInterface`1", "T", "in"))
                (withProjects genericModifiers, ("ContravariantGenericInterface`1", "T", "out"))]
                (fun sut project (name, parameter, modifier) () ->
                    let expected = normalizeSyntax modifier
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                        
                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Modifier = Some expected @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (
                ProjectLanguage.CSharp,
                ["
                    public interface SingleGenericInterface<T> { }

                    public interface GenericConstrainedInterface<T, Y>
                        where T : struct
                        where Y : IEnumerable, class, new() { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Interface SingleGenericInterface(Of T)
                    End Interface

                    Public Interface GenericConstrainedInterface(Of T As Structure, Y As { IEnumerable, Class, New })
                    End Interface
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no constraints when interface generic parameter has none"
                [withProjects genericConstraints, ("SingleGenericInterface`1", "T")]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints
                                                       |> Seq.isEmpty @>)

            yield! testRepeatParameterized "should return constraints when interface generic parameter has some" [
                (withProjects genericConstraints, ("GenericConstrainedInterface`2", "T", ["struct"]))
                (withProjects genericConstraints, ("GenericConstrainedInterface`2", "Y", ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (name, parameter, constraints) () ->
                    let expected = constraints |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.toList
                                                       |> ((=) expected) @>)
        ]