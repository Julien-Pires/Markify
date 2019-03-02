namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateReturnType_Tests =   
    [<Tests>]
    let returnTypeTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public delegate void VoidDelegate();
                public delegate Int32 IntegerDelegate();
                public delegate T GenericDelegate<T>();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub VoidDelegate()
                Public Delegate Function IntegerDelegate() As Int32
                Public Delegate Function GenericDelegate(Of T)() As T
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should return expected delegate return type" [
                ((withProjects content, ("VoidDelegate", "void")))
                ((withProjects content, ("IntegerDelegate", "Int32")))
                ((withProjects content, ("GenericDelegate`1", "T")))]
                (fun sut project (name, expected) () -> 
                    let result = sut.Analyze project |> findDelegate name
                        
                    test <@ result.ReturnType |> normalizeSyntax = expected @>)
        ]