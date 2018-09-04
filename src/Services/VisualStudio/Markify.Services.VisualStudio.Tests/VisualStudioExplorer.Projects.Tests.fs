namespace Markify.Services.VisualStudio.Tests

open System
open System.IO
open Expecto
open Swensen.Unquote
open Fixtures
open Markify.Domain.Ide
open EnvDTE

module VisualStudioExplorerProjectsTests =
    [<Tests>]
    let currentSolutionTests =
        testList "Projects" [
            yield! testFixture (withSolution defaultSolution) [
                "should returns an empty list when solution has no projects",
                fun sut ->
                    test <@ sut.Projects |> Seq.isEmpty  @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 2; Folders = 2; Depth = 2 }) [
                "should not has an empty project list when solution has some",
                fun sut ->
                    test <@ sut.Projects |> Seq.length = 14 @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 1 }) [
                "should has projects with valid name",
                fun sut ->
                    let result = sut.Projects |> Seq.head

                    test <@ result.Name |> String.IsNullOrWhiteSpace |> not @>
                
                "should has projects with no files when projects are empty",
                fun sut ->
                    let result = sut.Projects |> Seq.head

                    test <@ result.Files |> Seq.isEmpty @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 1; Project = { defaultProject with Files = 2; Folders = 2; Depth = 2; Extension = "cs" }}) [
                "should has projects with files when projects are not empty",
                fun sut ->
                    let result = sut.Projects |> Seq.head

                    test <@ result.Files |> Seq.length = 14 @>

                "should has projects with valid file path when projects are not empty",
                fun sut ->
                    let result = sut.Projects |> Seq.head

                    test <@ result.Files |> Seq.forall(fun d ->
                                            result.Path.IsBaseOf(d) && Path.GetExtension(d.AbsolutePath) = ".cs") @>
            ]

            yield! [
                let verifyProjectsLanguage expected (sut : IIDEExplorer) =
                    let result = sut.Projects |> Seq.head

                    test <@ result.Language = expected @>
                
                let withSolution language = 
                    withSolution { defaultSolution with Projects = 1; Project = { defaultProject with Language = language }}

                yield testCase "should has projects with unsupported language when project language is not supported" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageVC (verifyProjectsLanguage ProjectLanguage.Unsupported))

                yield testCase "should has projects with CSharp language" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageCSharp (verifyProjectsLanguage ProjectLanguage.CSharp))

                yield testCase "should has projects with Visual Basic language" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageVB (verifyProjectsLanguage ProjectLanguage.VisualBasic)) 
            ]
        ]