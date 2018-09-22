namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Ide
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures
open TestHelper

module RoslynAnalyzerTypesGenericsTests =
    [<Tests>]
    let analyzeTests =
        testList "Analyze" [
            yield! testRepeat (withProject "Generics") allLanguages [
                yield! testTheory "should return no generic parameters when type has none"
                    ["NoGenericType"]
                    (fun name project (sut: IProjectAnalyzer) ->
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies

                        test <@ result |> Seq.collect (fun c -> c.Identity.Parameters)
                                       |> Seq.isEmpty @>)

                yield! testTheory "should return generic parameters when type has some" 
                    [("SingleGenericType`1", 1); 
                    ("MultipleGenericType`2", 2)]
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, expected = parameters
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies
                        
                        test <@ result |> Seq.map (fun c -> Seq.length c.Identity.Parameters)
                                       |> Seq.forAllStrict ((=) expected) @>)

                yield! testTheory "should return valid generic parameter name when type has some" 
                    [("SingleGenericType`1", "T");
                    ("MultipleGenericType`2", "T");
                    ("MultipleGenericType`2", "Y")]
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, expected = parameters
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies
                        
                        test <@ result |> Seq.map (fun c -> c.Identity.Parameters)
                                       |> Seq.forAllStrict (fun c -> c |> Seq.exists (fun d -> d.Name = expected)) @>)
                
                yield! testTheory "should return no modifiers when generic parameter has none"
                    [("SingleGenericType`1", "T")]
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, generic = parameters
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies

                        test <@ result |> getGenericParameter generic
                                       |> Seq.forAllStrict (fun c -> c.Modifier.IsNone) @>)

                yield! testTheory "should return modifier when generic parameter has one" 
                    [("CovariantGenericType`1", "T", "in");
                    ("ContravariantGenericType`1", "T", "out")]
                    
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, generic, modifier = parameters
                        let expected = LanguageHelper.getModifier project.Language modifier
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies
                        
                        test <@ result |> getGenericParameter generic
                                       |> Seq.forAllStrict (fun c -> c.Modifier = Some expected) @>)

                yield! testTheory "should return no constraints when generic parameter has none"
                    [("SingleGenericType`1", "T")]
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, generic = parameters
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies

                        test <@ result |> getGenericParameter generic
                                       |> Seq.forAllStrict (fun c -> Seq.isEmpty c.Constraints) @>)

                yield! testTheory "should return constraints when generic parameter has some"
                    [("MultipleGenericType`2", "T", ["struct"]);
                    ("MultipleGenericType`2", "Y", ["IEnumerable"; "class"; "new()"])]
                    (fun parameters project (sut: IProjectAnalyzer) ->
                        let name, generic, constraints = parameters
                        let expected = TestHelper.getModifiers project.Language constraints
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies

                        test <@ result |> getGenericParameter generic
                                       |> Seq.map (fun c -> Seq.toList c.Constraints)
                                       |> Seq.forAllStrict ((=) expected) @>)
            ]
        ]