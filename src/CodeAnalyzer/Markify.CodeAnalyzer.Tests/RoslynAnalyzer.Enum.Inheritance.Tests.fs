namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_EnumInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (CSharp, ["
                public enum InheritPrimitiveType : Int32 { }
            "])
            (VisualBasic, ["
                Public Enum InheritPrimitiveType As Int32
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return base type when enum inherits from a primitive primitive"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "InheritPrimitiveType"
                        
                    test <@ result.BaseType |> Seq.contains "Int32" @>)
        ]