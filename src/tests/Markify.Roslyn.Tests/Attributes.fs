module Attributes

    open System
    open Customization

    open Ploeh.AutoFixture
    open Ploeh.AutoFixture.Xunit2

    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
    type ProjectContextInlineAutoDataAttribute(sourceFiles : string[], [<ParamArray>] values) =
        inherit InlineAutoDataAttribute(
            AutoDataAttribute(Fixture().Customize(ProjectContextCustomization(sourceFiles))),
            values)