namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesDelegateGenericsTests =
    [<Tests>]
    let noGenericDelegateTests =
        let noGeneric = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void NoGenericDelegate();
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Delegate Sub NoGenericDelegate()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no generic parameters when delegate has none"
                [(withProjects noGeneric, "NoGenericDelegate")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericDelegateTests =
        let generic = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void SingleGenericDelegate<T>();

                    public delegate void MultipleGenericDelegate<T, Y>();
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Delegate Sub SingleGenericDelegate(Of T)()

                    Public Delegate Sub MultipleGenericDelegate(Of T, Y)()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return generic parameters when delegate has some" [
                (withProjects generic, ("SingleGenericDelegate`1", 1))
                (withProjects generic, ("MultipleGenericDelegate`2", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name 
                        
                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized "should return valid generic parameter name when delegate has some" [
                (withProjects generic, ("SingleGenericDelegate`1", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "T"))
                (withProjects generic, ("MultipleGenericDelegate`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name
                        
                    test <@ result.Identity.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void SingleGenericDelegate<T>();

                    public delegate void CovariantGenericDelegate<in T>();

                    public delegate void ContravariantGenericDelegate<out T>();
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Delegate Sub SingleGenericDelegate(Of T)()

                    Public Delegate Sub CovariantGenericDelegate(Of In T)()

                    Public Delegate Sub ContravariantGenericDelegate(Of Out T)()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no modifiers when delegate generic parameter has none"
                [withProjects genericModifiers, ("SingleGenericDelegate`1", "T")]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Modifier.IsNone @>)

            yield! testRepeatParameterized "should return modifier when delegate generic parameter has one"  [
                (withProjects genericModifiers, ("CovariantGenericDelegate`1", "T", "in"))
                (withProjects genericModifiers, ("ContravariantGenericDelegate`1", "T", "out"))]
                (fun sut project (name, parameter, modifier) () ->
                    let expected = normalizeSyntax modifier
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name
                        
                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Modifier = Some expected @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void SingleGenericDelegate<T>();

                    public delegate void GenericConstrainedDelegate<T, Y>()
                        where T : struct
                        where Y : IEnumerable, class, new();
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Delegate Sub SingleGenericDelegate(Of T)()

                    Public Delegate Sub GenericConstrainedDelegate(Of T As Structure, Y As { IEnumerable, Class, New })()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized "should return no constraints when interface generic parameter has none"
                [withProjects genericConstraints, ("SingleGenericDelegate`1", "T")]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints
                                                       |> Seq.isEmpty @>)

            yield! testRepeatParameterized "should return constraints when interface generic parameter has some" [
                (withProjects genericConstraints, ("GenericConstrainedDelegate`2", "T", ["struct"]))
                (withProjects genericConstraints, ("GenericConstrainedDelegate`2", "Y", ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (name, parameter, constraints) () ->
                    let expected = constraints |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.toList
                                                       |> ((=) expected) @>)
        ]