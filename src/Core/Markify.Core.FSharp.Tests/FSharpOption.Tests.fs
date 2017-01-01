namespace Markify.Core.FSharp.Tests

open Markify.Core.FSharp
open Swensen.Unquote
open Xunit

module FSharpOption_Tests =

    [<Fact>]
    let ``IsSome should return true when option has value`` () =
        test <@ FSharpOptionExtension.IsSome (Some "Foo") @>

    [<Fact>]
    let ``IsSome should return false when option has no value`` () =
        test <@ not <| FSharpOptionExtension.IsSome None @>

    [<Fact>]
    let ``IsNone should return true when option has no value`` () =
        test <@ FSharpOptionExtension.IsNone None @>

    [<Fact>]
    let ``IsNone should return false when option has value`` () =
        test <@ not <| FSharpOptionExtension.IsNone (Some "Foo") @>

    [<Fact>]
    let ``Match should execute some action when option has value`` () =
        let actual = FSharpOptionExtension.Match (Some "Foo", (fun c -> true), (fun () -> false))

        test <@ actual @>

    [<Fact>]
    let ``Match should execute none action when option has no value`` () =
        let actual = FSharpOptionExtension.Match(None, (fun c -> false), (fun () -> true))

        test <@ actual @>