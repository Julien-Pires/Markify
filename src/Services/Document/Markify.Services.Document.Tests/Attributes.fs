namespace Markify.Services.Document.Tests

module Attributes =
    open System
    open Customization
    open Ploeh.AutoFixture
    open Ploeh.AutoFixture.Xunit2

    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
    type DocumentDataAttribute(projectName, projectCount, typeCount, extension, [<ParamArray>] values) =
        inherit InlineAutoDataAttribute(
            AutoDataAttribute(Fixture().Customize(SimpleDocumentCustomization(projectName, projectCount, typeCount, extension))),
            values)