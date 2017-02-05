namespace Markify.Services.Roslyn.Tests

open System
open Markify.Roslyn.Tests
open Ploeh.AutoFixture
open Ploeh.AutoFixture.Xunit2

module FixtureBuilder =
    let fixture file language = 
        Fixture()
        |> fun c -> c.Customize(ProjectContextCustomization(file, language))
        |> fun c -> 
            c.Customizations.Add(LanguageAnalyzerArgs())
            c

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type MultiProjectDataAttribute(solutionFile : string, language, [<ParamArray>] values) =
    inherit InlineAutoDataAttribute(
        AutoDataAttribute((FixtureBuilder.fixture solutionFile language)),
        values)