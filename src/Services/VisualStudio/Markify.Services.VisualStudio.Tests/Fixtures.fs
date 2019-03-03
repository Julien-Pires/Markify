namespace Markify.Services.VisualStudio.Tests

open Markify.Domain.Ide
open Markify.Services.VisualStudio
open EnvDTE

module Fixtures =
    let defaultProject = {
        Folders = 0
        Files = 0
        Depth = 0
        Language = CodeModelLanguageConstants.vsCMLanguageCSharp
        Extension = "cs" }

    let defaultSolution = {
        RootPath = "c:/Solution"
        Folders = 0
        Projects = 0 
        Depth = 0
        Project = defaultProject }

    let private createSut visualStudio = VisualStudioExplorer(visualStudio) :> IIDEExplorer

    let withNoSolution f () =
        f <| createSut (VisualStudioBuilder.create { Solution = None })

    let withSolution configuration f () =
        f <| createSut (VisualStudioBuilder.create { Solution = Some configuration })