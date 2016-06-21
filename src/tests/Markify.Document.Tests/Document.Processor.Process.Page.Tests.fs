module Document_Processor_Process_Page_Tests

    open System
    open Attributes
    open Markify.Core.Processors

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 1, 2, "md", 1, "Type1.md")>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 4, 2, "md", 4, "Type0.md")>]
    let ``Process library should return page with correct name`` (expected, name, setting, solution, library, sut : IDocumentOrganizer) =
        let toc = sut.Organize (library, solution, setting)
        let actual =
            toc.Pages
            |> Seq.filter (fun c -> c.Name = name)

        test <@ Seq.length actual = expected @>

    [<Theory>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 1, 1, "md", "Foo.md")>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 4, 1, "md", "Bar.md")>]
    let ``Process library should not return page with incorrect name`` (expected, setting, solution, library, sut : IDocumentOrganizer) =
        let toc = sut.Organize (library, solution, setting)
        let actual =
            toc.Pages
            |> Seq.tryFind (fun c -> c.Name = expected)

        test <@ actual.IsNone @>

    [<Theory>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 1, 2, "md", @"FooProject0\Parent\", 2)>]
    [<SimpleDocumentInlineAutoData("c:/FooSolution/", "FooProject", 4, 4, "md", @"FooProject2\Parent\", 4)>]
    let ``Process library should return page with correct path`` (path, expected, setting, solution, library, sut : IDocumentOrganizer) =
        let toc = sut.Organize (library, solution, setting)
        let actual =
            toc.Pages
            |> Seq.filter (fun c -> c.Folder = new Uri(path, UriKind.Relative))

        test <@ Seq.length actual = expected @>