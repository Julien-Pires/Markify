namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateGenerics_Tests =
    [<Tests>]
    let noGenericDelegateTests =
        let noGeneric = [
            (CSharp, ["
                public delegate void NoGenericDelegate();
            "])
            (VisualBasic, ["
                Public Delegate Sub NoGenericDelegate()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects noGeneric)
                "should return no generic parameters when delegate has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoGenericDelegate"

                    test <@ result.Generics |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericDelegateTests =
        let generic = [
            (CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void MultipleGenericDelegate<T, Y>();
            "])
            (VisualBasic, ["
                Public Delegate Sub SingleGenericDelegate(Of T)()
                Public Delegate Sub MultipleGenericDelegate(Of T, Y)()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized 
                "should return generic parameters when delegate has some" [
                (withProjects generic, ("SingleGenericDelegate`1", 1))
                (withProjects generic, ("MultipleGenericDelegate`2", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when delegate has some" [
                (withProjects generic, ("SingleGenericDelegate`1", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void CovariantGenericDelegate<in T>();
                public delegate void ContravariantGenericDelegate<out T>();
            "])
            (VisualBasic, ["
                Public Delegate Sub SingleGenericDelegate(Of T)()
                Public Delegate Sub CovariantGenericDelegate(Of In T)()
                Public Delegate Sub ContravariantGenericDelegate(Of Out T)()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects genericModifiers) 
                "should return no modifiers when delegate generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericDelegate`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Modifier.IsNone @>)

            yield! testRepeatParameterized 
                "should return modifier when delegate generic parameter has one"  [
                (withProjects genericModifiers, ("CovariantGenericDelegate`1", "T", "in"))
                (withProjects genericModifiers, ("ContravariantGenericDelegate`1", "T", "out"))]
                (fun sut project (name, parameter, expected) () ->
                    let definition = sut.Analyze project |> findType name
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)
                        
                    test <@ result.Modifier.Value |> normalizeSyntax = expected @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void GenericConstrainedDelegate<T, Y>()
                    where T : struct
                    where Y : IEnumerable, class, new();
            "])
            (VisualBasic, ["
                Public Delegate Sub SingleGenericDelegate(Of T)()
                Public Delegate Sub GenericConstrainedDelegate(Of T As Structure, Y As { IEnumerable, Class, New })()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects genericConstraints)
                "should return no constraints when delegate generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericDelegate`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when delegate generic parameter has some" [
                (withProjects genericConstraints, ("T", Set ["struct"]))
                (withProjects genericConstraints, ("Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (parameter, expected) () ->
                    let definition = sut.Analyze project |> findType "GenericConstrainedDelegate`2"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Set
                                               |> Set.map normalizeSyntax
                                               |> (=) expected @>)
        ]