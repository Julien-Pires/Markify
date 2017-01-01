module IOTests
    open System
    open System.IO
    open System.Reflection
    open Markify.Core.FSharp.IO
    open Markify.Core.FSharp.Builders

    open Xunit
    open Swensen.Unquote

    let isSuccess = function
        | Success _ -> true
        | _ -> false

    let isFailure  = function
        | Failure _ -> true
        | _ -> false

    let getFailure = function
        | Failure x -> Some x
        | _ -> None

    [<Theory>]
    [<InlineData("", false)>]
    [<InlineData("Foo.txt", true)>]
    let ``FileExists should return expected value`` (path, expected) =
        let actual = IO.fileExists path

        test <@ Success expected = actual @>

    [<Theory>]
    [<InlineData("Foo.txt")>]
    let ``ReadFile should return success when file exists`` (path) =
        let actual = IO.readFile path

        test <@ actual |> isSuccess @>

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("$ù!ù:@@")>]
    [<InlineData("Bar.txt")>]
    [<InlineData("Foo/Bar.txt")>]
    let ``ReadFile should return failure when path is incorrect`` (path) =
        let actual = IO.readFile path

        test <@ actual |> isFailure @>
        test <@ actual |> getFailure |> fun c -> String.IsNullOrWhiteSpace (c.Value.Message) = false @>
        test <@ actual |> getFailure |> fun c -> String.IsNullOrWhiteSpace (c.Value.Stack) = false @>