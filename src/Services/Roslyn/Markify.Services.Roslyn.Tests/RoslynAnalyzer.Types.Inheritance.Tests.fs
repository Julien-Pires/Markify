namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Ide
open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesInheritanceTests =
    [<Tests>]
    let analyzeTests =
        testList "Analyze" [
            yield! testRepeat (withProject "Inheritance") allLanguages [
                yield! testTheory "should return base types when type inherits from other types"
                    [("InheritType", ["Exception"]);
                    ("InheritPrimitiveType", ["Int32"]);
                    ("ImplementInterfaceType", ["IDisposable"]);
                    ("MixedInheritanceType", ["Exception"; "IDisposable"])]
                    (fun parameters project (sut : IProjectAnalyzer) ->
                        let name, expected = parameters
                        let assemblies = sut.Analyze project
                        let result = TestHelper.filterTypes name assemblies
                        
                        test <@ result |> Seq.map (fun c -> Set c.Identity.BaseTypes)
                                       |> Seq.forAllStrict ((Set.isSubset) (Set expected)) @>)
            ]
        ]