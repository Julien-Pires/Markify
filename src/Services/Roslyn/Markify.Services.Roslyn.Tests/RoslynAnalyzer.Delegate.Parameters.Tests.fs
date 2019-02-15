namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateParameters_Tests =
    [<Tests>]
    let noParametersTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithoutParameters();
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithoutParameters()
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no parameters when delegate has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "WithoutParameters"

                    test <@ result.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withParametersTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithOneParameter(Int32 foo);
                    public delegate void WithMultipleParameters(Int32 foo, Single bar);
                    public delegate void WithGenericParameters<T>(T foo, T[] bar);
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithOneParameter(foo As Int32)
                    Public Delegate Sub WithMultipleParameters(foo As Int32, bar As Single)
                    Public Delegate Sub WithGenericParameters(Of T)(foo As T, bar As T())
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should return parameters when delegate has some"[
                ((withProjects content), ("WithOneParameter", 1))
                ((withProjects content), ("WithMultipleParameters", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameters name when delegate has some"[
                ((withProjects content), ("WithOneParameter", "foo"))
                ((withProjects content), ("WithMultipleParameters", "foo"))
                ((withProjects content), ("WithMultipleParameters", "bar"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct parameters return type when delegate has some"[
                ((withProjects content), ("WithOneParameter", "foo", "Int32"))
                ((withProjects content), ("WithMultipleParameters", "foo", "Int32"))
                ((withProjects content), ("WithMultipleParameters", "bar", "Single"))
                ((withProjects content), ("WithGenericParameters`1", "foo", "T"))]
                (fun sut project (name, parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let noParameterModifierTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithOneParameter(Int32 foo);
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithOneParameter(foo As Int32)
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no modifier when delegate parameter has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "WithOneParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "Foo")
                                              |> fun c -> c.Modifier.IsNone @>)
        ]

    [<Tests>]
    let withParameterModifierTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithParametersModifiers(ref Int32 foo, out Int32 bar);
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithParametersModifiers(ByRef foo As Int32, ByVal bar As Single)
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should return modifier when delegate parameter has one" [
                ((withProjects content), ("WithParametersModifiers", "foo", "ref"))
                ((withProjects content), ("WithParametersModifiers", "bar", "out"))]
                (fun sut project (name, parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Modifier = Some expected @>)
        ]
    
    [<Tests>]
    let noParameterDefaultValueTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithOneParameter(Int32 foo);
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithOneParameter(foo As Int32)
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no default value when delegate parameter has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "WithOneParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "Foo")
                                              |> fun c -> c.DefaultValue.IsNone @>)
        ]

    [<Tests>]
    let withParameterDefaultValueTests =
        let content = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void WithDefaultValueParameter(Int32 foo = 1);
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub WithDefaultValueParameter(Optional foo As Int32 = 1)
                "]
            )
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return default value when delegate parameter has one"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "WithDefaultValueParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "Foo")
                                              |> fun c -> c.DefaultValue = Some "1" @>)
        ]