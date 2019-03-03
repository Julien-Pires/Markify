namespace Markify.Services.VisualStudio.Tests

open System
open System.IO
open EnvDTE
open Expecto
open Swensen.Unquote
open Fixtures
open Markify.Domain.Ide

module VisualStudioExplorerActiveProjectTests =
    [<Tests>]
    let activeProjectTests =
        testList "ActiveProject" [
            yield! testFixture (withSolution defaultSolution) [
                "should returns none when user has selected no project",
                fun sut -> 
                    test <@ Option.isNone sut.ActiveProject @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 2 }) [
                "should returns a project when user has selected one",
                fun sut -> 
                    test <@ Option.isSome sut.ActiveProject @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 1 }) [
                "should has valid name",
                fun sut ->
                    let result = sut.ActiveProject.Value

                    test <@ result.Name |> String.IsNullOrWhiteSpace |> not @>
                
                "should has no files when project is empty",
                fun sut ->
                    let result = sut.Projects |> Seq.head

                    test <@ result.Files |> Seq.isEmpty @>
            ]

            yield! testFixture (withSolution { defaultSolution with Projects = 1; Project = { defaultProject with Files = 2; Folders = 2; Depth = 2; Extension = "cs" }}) [
                "should has files when project is not empty",
                fun sut ->
                    let result = sut.ActiveProject.Value

                    test <@ result.Files |> Seq.length = 14 @>

                "should has valid file path when project is not empty",
                fun sut ->
                    let result = sut.ActiveProject.Value

                    test <@ result.Files |> Seq.forall(fun d ->
                                            result.Path.IsBaseOf(d) && Path.GetExtension(d.AbsolutePath) = ".cs") @>
            ]

            yield! [
                let verifyProjectsLanguage expected (sut : IIDEExplorer) =
                    let result = sut.ActiveProject.Value

                    test <@ result.Language = expected @>
                
                let withSolution language = 
                    withSolution { defaultSolution with Projects = 1; Project = { defaultProject with Language = language }}

                yield testCase "should has unsupported language when project language is not supported" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageVC (verifyProjectsLanguage ProjectLanguage.Unsupported))

                yield testCase "should has CSharp language" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageCSharp (verifyProjectsLanguage ProjectLanguage.CSharp))

                yield testCase "should has Visual Basic language" <|
                    (withSolution CodeModelLanguageConstants.vsCMLanguageVB (verifyProjectsLanguage ProjectLanguage.VisualBasic)) 
            ]
        ]