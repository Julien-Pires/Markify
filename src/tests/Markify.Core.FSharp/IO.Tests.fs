module IO_Tests
    open Markify.Core.IO

    open Xunit
    open Swensen.Unquote

    let isSuccess ioResult =
        match ioResult with
        | Success s -> true
        | _ -> false

    let isIOError ioResult =
        match ioResult with
        | Failure f ->
            match f with
            | Error e -> true
            | _ -> false
        | _ -> false

    let isIOException ioResult = 
        match ioResult with
        | Failure f ->
            match f with
            | Exception e -> true
            | _ -> false
        | _ -> false

    [<Theory>]
    [<InlineData("", false)>]
    [<InlineData("foo.txt", false)>]
    [<InlineData("Foo/Bar/FooBar.txt", false)>]
    [<InlineData("Pojects/Source/EmptySource.cs", true)>]
    let ``Detect file exists with correct value`` (path, exists) =
        let actual = IO.fileExists path

        test <@ Success exists = actual @>

    [<Theory>]
    [<InlineData("Pojects/Class_Project.xml")>]
    [<InlineData("Pojects/Source/EmptySource.cs")>]
    let ``Read text file when file exists with success`` (path) =
        let actual = IO.readFile path

        test <@ true = isSuccess actual @>

    [<Theory>]
    [<InlineData("Foo.txt")>]
    [<InlineData("Foo/Bar.xml")>]
    let ``Read text file when file does not exists with failure value`` (path) =
        let actual = IO.readFile path

        test <@ true = isIOError actual @>

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("$ù!ù:@@")>]
    let ```Read text file when using wrong parameters with exception value`` (path) =
        let actual = IO.readFile path

        test <@ true = isIOException actual @>