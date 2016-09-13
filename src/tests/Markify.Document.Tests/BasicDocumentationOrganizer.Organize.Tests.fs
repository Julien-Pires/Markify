module BasicDocumentationOrganizer_Tests

    open System
    open Attributes
    open Markify.Core.Analyzers
    open Markify.Document
    open Xunit
    open Swensen.Unquote
    
    [<Theory>]
    [<DocumentData("FooProject", 0, 0, "md", 0)>]
    [<DocumentData("FooProject", 1, 10, "md", 10)>]
    [<DocumentData("FooProject", 10, 10, "md", 100)>]
    let ``Organize should return correct pages count`` (expected, setting, root, library, sut : BasicDocumentationOrganizer) =
        let actual = (sut :> IDocumentationOrganizer).Organize (library, root, setting)

        test <@ Seq.length actual.Pages = expected @>

    [<Theory>]
    [<DocumentData("FooProject", 0, 0, "md")>]
    [<DocumentData("FooProject", 0, 0, "md")>]
    let ``Organize should return correct path`` (setting, root, library, sut : BasicDocumentationOrganizer) =
        let actual = (sut :> IDocumentationOrganizer).Organize (library, root, setting)

        test <@ actual.Root = Uri(root, "/docs/") @>

    [<Theory>]
    [<DocumentData("FooProject", 1, 2, "md", 1, "Type1.md")>]
    [<DocumentData("FooProject", 4, 2, "md", 4, "Type0.md")>]
    let ``Organize should return page with correct name`` (expected, name, setting, root, library, sut : BasicDocumentationOrganizer) =
        let toc = (sut :> IDocumentationOrganizer).Organize (library, root, setting)
        let actual = toc.Pages |> Seq.filter (fun c -> c.Name = name)

        test <@ Seq.length actual = expected @>

    [<Theory>]
    [<DocumentData("FooProject", 1, 1, "md", "Foo.md")>]
    [<DocumentData("FooProject", 4, 1, "md", "Bar.md")>]
    let ``Organize should not return page with incorrect name`` (expected, setting, root, library, sut : BasicDocumentationOrganizer) =
        let toc = (sut :> IDocumentationOrganizer).Organize (library, root, setting)
        let actual = toc.Pages |> Seq.tryFind (fun c -> c.Name = expected)

        test <@ actual.IsNone @>

    [<Theory>]
    [<DocumentData("FooProject", 1, 2, "md", @"FooProject0\Parent\", 2)>]
    [<DocumentData("FooProject", 4, 4, "md", @"FooProject2\Parent\", 4)>]
    let ``Organize should return page with correct path`` (path, expected, setting, root, library, sut : BasicDocumentationOrganizer) =
        let toc = (sut :> IDocumentationOrganizer).Organize (library, root, setting)
        let actual = toc.Pages |> Seq.filter (fun c -> c.Folder = new Uri(path, UriKind.Relative))

        test <@ Seq.length actual = expected @>