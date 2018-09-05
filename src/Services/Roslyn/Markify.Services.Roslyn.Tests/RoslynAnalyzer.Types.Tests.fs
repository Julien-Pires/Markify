namespace Markify.Services.Roslyn.Tests

open Expecto
open Fixtures
open Swensen.Unquote
open Markify.Services.Roslyn
open Markify.Domain.Compiler
open TestHelper

module RoslynAnalyzerTypesTests =
    open Markify.Domain.Ide

    [<Tests>]
    let analyzeTests =
        testList "Analyze" [
            yield! testRepeat (withProject "Empty" [ProjectLanguage.CSharp; ProjectLanguage.VisualBasic]) [
                "should returns no type when project is empty",
                fun project sut ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty @>
            ]

            yield! testRepeat (withProject "Organization" [ProjectLanguage.CSharp; ProjectLanguage.VisualBasic]) [
                "should returns types when project is not empty",
                fun project (sut : IProjectAnalyzer) ->
                    let result = sut.Analyze project

                    test <@ result.Types |> Seq.isEmpty |> not @>
            ]

            yield! testRepeat (withProject "Organization" [ProjectLanguage.CSharp; ProjectLanguage.VisualBasic]) [
                yield! testTheory ["FooType"; "NestedType"; "DeeperNestedType"] [
                    "should return types with valid name",
                    fun name project (sut: IProjectAnalyzer) ->
                        let assemblies = sut.Analyze project
                        let result = getDefinitions name assemblies
                            
                        test <@ result |> Seq.length > 0 @> ]

                yield! testTheory ["FooType"; "ParentType.NestedType"; "ParentType.AnotherNestedType.DeeperNestedType"] [
                    "should return types with valid fullname",
                    fun fullname project (sut: IProjectAnalyzer) ->
                        let fullnames = NamespaceHelper.AllTypes |> Seq.map (fun c -> sprintf "%s.%s" c fullname)
                        let library = sut.Analyze project
                        let actual = fullnames |> Seq.map (fun c -> TestHelper.getDefinitionByFullname c library)

                        test <@ (actual |> Seq.length) = (fullnames |> Seq.length) @>]
            ]

            yield! testRepeat (withProject "Duplicates" [ProjectLanguage.CSharp; ProjectLanguage.VisualBasic]) [
                "should returns no duplicate types",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = assemblies.Types |> Seq.groupBy (fun c -> getFullname c.Identity)

                    test <@ (result |> Seq.length) = (assemblies.Types |> Seq.length) @>
            ]
        ]