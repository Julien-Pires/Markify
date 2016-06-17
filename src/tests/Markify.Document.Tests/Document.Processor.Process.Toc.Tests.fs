module Document_Processor_Process_Toc_Tests

    open System
    open Attributes
    open Markify.Core.Processors

    open Xunit
    open Swensen.Unquote
    
    [<Theory>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 0, 0, "md", 0)>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 1, 10, "md", 10)>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 10, 10, "md", 100)>]
    let ``Process library should return correct pages count`` (expected, setting, solution, library, sut : IDocumentOrganizer) =
        let actual = sut.Organize (library, solution, setting)

        test <@ Seq.length actual.Pages = expected @>

    [<Theory>]
    [<SimpleDocumentInlineAutoData("c:/", "FooProject", 0, 0, "md", "c:/docs/")>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 0, 0, "md", "c:/FooSolution/docs/")>]
    let ``Process library should return correct path`` (expected, setting, solution, library, sut : IDocumentOrganizer) =
        let actual = sut.Organize (library, solution, setting)

        test <@ actual.Root = new Uri(expected) @>