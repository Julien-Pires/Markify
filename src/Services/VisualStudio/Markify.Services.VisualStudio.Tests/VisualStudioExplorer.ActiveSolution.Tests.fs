namespace Markify.Services.VisualStudio.Tests

open System
open Expecto
open Swensen.Unquote
open Fixtures

module VisualStudioExplorerActiveSolutionTests =
    [<Tests>]
    let activeSolutionTests =
        testList "ActiveSolution" [
            yield! testFixture withNoSolution [
                "should returns none when no solution is loaded",
                fun sut -> 
                    test <@ Option.isNone sut.ActiveSolution @>
            ]

            yield! testFixture (withSolution { defaultSolution with RootPath = "c:/Solution" }) [
                "should returns a solution with a valid path",
                fun sut ->
                    test <@ sut.ActiveSolution.Value.Path = Uri("c:/Solution") @>
            ]
        ]