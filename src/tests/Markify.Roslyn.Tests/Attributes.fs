module Attributes

    open System

    open Markify.Roslyn.Tests

    open Ploeh.AutoFixture
    open Ploeh.AutoFixture.Xunit2

    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
    type ProjectContextInlineAutoDataAttribute(projectFile : string, language, [<ParamArray>] values) =
        inherit InlineAutoDataAttribute(
            AutoDataAttribute(Fixture().Customize(ProjectContextCustomization(projectFile, language))),
            values)