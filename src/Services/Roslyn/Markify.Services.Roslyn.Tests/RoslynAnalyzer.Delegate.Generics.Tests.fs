namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateGenerics_Tests =
    [<Tests>]
    let noGenericDelegateTests =
        let noGeneric = [
            (ProjectLanguage.CSharp, ["
                public delegate void NoGenericDelegate();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub NoGenericDelegate()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized 
                "should return no generic parameters when delegate has none" [
                (withProjects noGeneric, "NoGenericDelegate")]
                (fun sut project name () ->
                    let result = sut.Analyze project |> findDelegate name

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericDelegateTests =
        let generic = [
            (ProjectLanguage.CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void MultipleGenericDelegate<T, Y>();
            "])
            (ProjectLanguage.VisualBasic, ["
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
                    let result = sut.Analyze project |> findDelegate name
                        
                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when delegate has some" [
                (withProjects generic, ("SingleGenericDelegate`1", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findDelegate name
                        
                    test <@ result.Identity.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (ProjectLanguage.CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void CovariantGenericDelegate<in T>();
                public delegate void ContravariantGenericDelegate<out T>();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub SingleGenericDelegate(Of T)()
                Public Delegate Sub CovariantGenericDelegate(Of In T)()
                Public Delegate Sub ContravariantGenericDelegate(Of Out T)()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized 
                "should return no modifiers when delegate generic parameter has none" [
                (withProjects genericModifiers, ("SingleGenericDelegate`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let object = sut.Analyze project |> findDelegate name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Modifier.IsNone @>)

            yield! testRepeatParameterized 
                "should return modifier when delegate generic parameter has one"  [
                (withProjects genericModifiers, ("CovariantGenericDelegate`1", "T", "in"))
                (withProjects genericModifiers, ("ContravariantGenericDelegate`1", "T", "out"))]
                (fun sut project (name, parameter, expected) () ->
                    let object = sut.Analyze project |> findDelegate name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                        
                    test <@ result.Modifier.Value |> normalizeSyntax = expected @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (ProjectLanguage.CSharp, ["
                public delegate void SingleGenericDelegate<T>();
                public delegate void GenericConstrainedDelegate<T, Y>()
                    where T : struct
                    where Y : IEnumerable, class, new();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub SingleGenericDelegate(Of T)()
                Public Delegate Sub GenericConstrainedDelegate(Of T As Structure, Y As { IEnumerable, Class, New })()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized 
                "should return no constraints when delegate generic parameter has none" [
                (withProjects genericConstraints, ("SingleGenericDelegate`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let object = sut.Analyze project |> findDelegate name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when delegate generic parameter has some" [
                (withProjects genericConstraints, ("GenericConstrainedDelegate`2", "T", Set ["struct"]))
                (withProjects genericConstraints, ("GenericConstrainedDelegate`2", "Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (name, parameter, expected) () ->
                    let object = sut.Analyze project |> findDelegate name
                    let result = object.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Seq.map normalizeSyntax
                                               |> Set
                                               |> Set.isSubset expected @>)
        ]