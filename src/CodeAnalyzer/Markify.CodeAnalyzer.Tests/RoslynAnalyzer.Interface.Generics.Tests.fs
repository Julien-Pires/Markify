namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceGenerics_Tests =
    [<Tests>]
    let noGenericInterfaceTests =
        let noGeneric = [
            (CSharp, ["
                public interface NoGenericInterface { }
            "])
            (VisualBasic, ["
                Public Interface NoGenericInterface
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects noGeneric) 
                "should return no generic parameters when interface has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoGenericInterface"

                    test <@ result.Generics |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericInterfaceTests =
        let generic = [
            (CSharp, ["
                public interface SingleGenericInterface<T> { }
                public interface MultipleGenericInterface<T, Y> { }
            "])
            (VisualBasic, ["
                Public Interface SingleGenericInterface(Of T)
                End Interface
                Public Interface MultipleGenericInterface(Of T, Y)
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized 
                "should return generic parameters when interface has some" [
                (withProjects generic, ("SingleGenericInterface`1", 1))
                (withProjects generic, ("MultipleGenericInterface`2", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when interface has some" [
                (withProjects generic, ("SingleGenericInterface`1", "T"))
                (withProjects generic, ("MultipleGenericInterface`2", "T"))
                (withProjects generic, ("MultipleGenericInterface`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (CSharp, ["
                public interface SingleGenericInterface<T> { }
                public interface CovariantGenericInterface<in T> { }
                public interface ContravariantGenericInterface<out T> { }
            "])
            (VisualBasic, ["
                Public Interface SingleGenericInterface(Of T)
                End Interface
                Public Interface CovariantGenericInterface(Of In T)
                End Interface
                Public Interface ContravariantGenericInterface(Of Out T)
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects genericModifiers)
                "should return no modifiers when interface generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericInterface`1" 
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Modifier.IsNone @>)

            yield! testRepeatParameterized 
                "should return modifier when interface generic parameter has one"  [
                (withProjects genericModifiers, ("CovariantGenericInterface`1", "T", "in"))
                (withProjects genericModifiers, ("ContravariantGenericInterface`1", "T", "out"))]
                (fun sut project (name, parameter, expected) () ->
                    let definition = sut.Analyze project |> findType name 
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)
                        
                    test <@ result.Modifier.Value |> normalizeSyntax = expected @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (CSharp, ["
                public interface SingleGenericInterface<T> { }
                public interface GenericConstrainedInterface<T, Y>
                    where T : struct
                    where Y : IEnumerable, class, new() { }
            "])
            (VisualBasic, ["
                Public Interface SingleGenericInterface(Of T)
                End Interface
                Public Interface GenericConstrainedInterface(Of T As Structure, Y As { IEnumerable, Class, New })
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects genericConstraints)
                "should return no constraints when interface generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericInterface`1" 
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when interface generic parameter has some" [
                (withProjects genericConstraints, ("T", Set ["struct"]))
                (withProjects genericConstraints, ("Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (parameter, expected) () ->
                    let definition = sut.Analyze project |> findType "GenericConstrainedInterface`2" 
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Set
                                               |> Set.map normalizeSyntax
                                               |> (=) expected @>)
        ]