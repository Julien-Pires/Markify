module Attributes

    open System
    open Customization

    open Ploeh.AutoFixture
    open Ploeh.AutoFixture.Xunit2

    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
    type SimpleDocumentInlineAutoDataAttribute(root, projectName, projectCount, typeCount, extension, [<ParamArray>] values) =
        inherit InlineAutoDataAttribute(
            AutoDataAttribute(Fixture().Customize(SimpleDocumentCustomization(root, projectName, projectCount, typeCount, extension))),
            values
        )