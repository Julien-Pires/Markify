namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateParameters_Tests =
    [<Tests>]
    let noParametersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithoutParameters();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithoutParameters()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no parameters when delegate has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "WithoutParameters"

                    test <@ result.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withParametersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithOneParameter(Int32 A);
                public delegate void WithMultipleParameters(Int32 A, Single B);
                public delegate void WithGenericParameters<T>(T A, T[] B);
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithOneParameter(A As Int32)
                Public Delegate Sub WithMultipleParameters(A As Int32, B As Single)
                Public Delegate Sub WithGenericParameters(Of T)(A As T, B As T())
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should return parameters when delegate has some"[
                ((withProjects content), ("WithOneParameter", 1))
                ((withProjects content), ("WithMultipleParameters", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findDelegate name

                    test <@ result.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameters name when delegate has some"[
                ((withProjects content), ("WithOneParameter", "A"))
                ((withProjects content), ("WithMultipleParameters", "A"))
                ((withProjects content), ("WithMultipleParameters", "B"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findDelegate name

                    test <@ result.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct parameters return type when delegate has some"[
                ((withProjects content), ("WithOneParameter", "A", "Int32"))
                ((withProjects content), ("WithMultipleParameters", "A", "Int32"))
                ((withProjects content), ("WithMultipleParameters", "B", "Single"))
                ((withProjects content), ("WithGenericParameters`1", "A", "T"))]
                (fun sut project (name, parameter, expected) () ->
                    let result = sut.Analyze project |> findDelegate name

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Type = expected @>)
        ]

    [<Tests>]
    let noParameterModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithOneParameter(Int32 A);
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithOneParameter(A As Int32)
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no modifier when delegate parameter has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "WithOneParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.Modifier.IsNone @>)
        ]

    [<Tests>]
    let withParameterModifierTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithParametersModifiers(ref Int32 A, out Int32 B);
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithParametersModifiers(ByRef A As Int32, ByVal B As Single)
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should return modifier when delegate parameter has one" [
                ((withProjects content), ("WithParametersModifiers", "A", "ref"))
                ((withProjects content), ("WithParametersModifiers", "B", "out"))]
                (fun sut project (name, parameter, expected) () ->
                    let result = sut.Analyze project |> findDelegate name

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> normalizeSyntax c.Modifier.Value = expected @>)
        ]
    
    [<Tests>]
    let noParameterDefaultValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithOneParameter(Int32 A);
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithOneParameter(A As Int32)
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return no default value when delegate parameter has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "WithOneParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.DefaultValue.IsNone @>)
        ]

    [<Tests>]
    let withParameterDefaultValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void WithDefaultValueParameter(Int32 A = 1);
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub WithDefaultValueParameter(Optional A As Int32 = 1)
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects content)
                "should return default value when delegate parameter has one"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "WithDefaultValueParameter"

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.DefaultValue = Some "1" @>)
        ]