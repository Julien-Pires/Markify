namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_EnumValues_Tests =
    [<Tests>]
    let noValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public enum Empty { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Enum Empty
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects content)
                "should return no values when enum has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies "Empty"
                    
                    test <@ result.Values |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public enum SingleValue 
                {
                    A
                }
                public enum MultipleValues 
                {
                    A,
                    B
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Enum SingleValue
                    A
                End Enum
                Public Enum MultipleValues
                    A
                    B
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeatParameterized
                "should return values when enum has some"[
                (withProjects content, ("SingleValue", 1))
                (withProjects content, ("MultipleValues", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies name
                    
                    test <@ result.Values |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct value name when enum has some"[
                (withProjects content, ("SingleValue", "A"))
                (withProjects content, ("MultipleValues", "B"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies name
                    
                    test <@ result.Values |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let underlyingValueTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public enum WithoutUnderlyingValues 
                {
                    A,
                    B
                }
                public enum WithUnderlyingValues : Int32
                {
                    A = 0,
                    B = 1
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Enum WithoutUnderlyingValues
                    A
                    B
                End Enum
                Public Enum WithUnderlyingValues As Int32
                    A = 0
                    B = 1
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeatParameterized
                "should return no underlying value when enum values has none"[
                (withProjects content, ("WithoutUnderlyingValues", "A"))
                (withProjects content, ("WithoutUnderlyingValues", "B"))]
                (fun sut project (name, value) () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies name
                    
                    test <@ result.Values |> Seq.find (fun c -> c.Name = value)
                                          |> fun c -> c.Value.IsNone @>)

            yield! testRepeatParameterized
                "should return underlying value when enum values has one"[
                (withProjects content, ("WithUnderlyingValues", "A", "0"))
                (withProjects content, ("WithUnderlyingValues", "B", "1"))]
                (fun sut project (name, value, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies name
                    
                    test <@ result.Values |> Seq.find (fun c -> c.Name = value)
                                          |> fun c -> c.Value = Some expected @>)
        ]