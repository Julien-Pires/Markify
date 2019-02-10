﻿namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_EnumInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public enum InheritPrimitiveType : Int32 { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Enum InheritPrimitiveType As Int32
                    End Enum
                "]
            )
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return base type when enum inherits from a primitive primitive"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies "InheritPrimitiveType"
                        
                    test <@ result.Identity.BaseTypes |> Seq.contains "Int32" @>)
        ]