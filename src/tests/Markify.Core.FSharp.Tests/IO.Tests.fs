module IOTests
    open System
    open System.IO
    open System.Reflection
    open Markify.Core.IO
    open Markify.Core.Builders

    open Xunit
    open Swensen.Unquote

    let getSuccess ioResult =
        match ioResult with
        | Success s -> Some s
        | _ -> None

    let getFailure ioResult = 
        match ioResult with
        | Failure f -> Some f
        | _ -> None

    let getFullpath path =
        let dir = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase)
        let cleanDir = Path.GetDirectoryName (Uri.UnescapeDataString dir.Path)
        Path.Combine (cleanDir, path)

    [<Theory>]
    [<InlineData("", false)>]
    [<InlineData("foo.txt", false)>]
    [<InlineData("Foo/Bar/FooBar.txt", false)>]
    [<InlineData("Projects/Source/EmptySource.cs", true)>]
    let ``Detect file exists with correct value`` (path, exists) =
        let actual = IO.fileExists path

        test <@ Success exists = actual @>

    [<Theory>]
    [<InlineData("Projects/ClassProject.xml")>]
    [<InlineData("Projects/Source/EmptySource.cs")>]
    let ``Read text file when file exists with success`` (path) =
        let actual = IO.readFile path

        test <@ (getSuccess actual).IsSome @>

    [<Theory>]
    [<InlineData("Foo.txt")>]
    [<InlineData("Foo/Bar.xml")>]
    let ``Read text file when file does not exists with failure value`` (path) =
        let actual = IO.readFile path
        let ex = getFailure actual

        test <@ ex.IsSome @>
        test <@ String.IsNullOrWhiteSpace ex.Value.Message = false @>
        test <@ String.IsNullOrWhiteSpace ex.Value.Stack = false @>

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("$ù!ù:@@")>]
    let ```Read text file when using wrong parameters with failure value`` (path) =
        let actual = IO.readFile path
        let ex = getFailure actual

        test <@ ex.IsSome @>
        test <@ String.IsNullOrWhiteSpace ex.Value.Message = false @>
        test <@ String.IsNullOrWhiteSpace ex.Value.Stack = false @>