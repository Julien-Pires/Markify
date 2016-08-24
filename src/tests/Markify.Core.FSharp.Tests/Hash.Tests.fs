module Hash_Tests
    open Markify.Core.Builders
    open Xunit
    open Swensen.Unquote

    let combineHash values prime hash =
        values
        |> Seq.fold (fun acc c -> 
            acc * prime + c.GetHashCode()
        ) hash

    let computeHashFromSeq prime values =
        combineHash values prime 1

    let computeHashFromOptionSeq prime values = 
        values
        |> Seq.fold (fun acc c ->
            match c with
            | None -> acc
            | Some d -> acc * prime + d.GetHashCode()
        ) 1

    [<Theory>]
    [<InlineDataAttribute(15)>]
    [<InlineDataAttribute(31)>]
    let ``Combine hashcode with values`` (prime) =
        let values = ["Foo"; "Bar"; "Foobar"]
        let expected = computeHashFromSeq prime values

        let builder = HashBuilder(prime)
        let result = builder {
            yield (values |> List.item 2)
            yield (values |> List.item 1)
            yield (values |> List.item 0)
        }
        let actual = result.Hash

        test <@ actual = expected @>

    [<Theory>]
    [<InlineDataAttribute(15)>]
    [<InlineDataAttribute(31)>]
    let ``Combine hashcode with option values`` (prime) =
        let values = [Some "Foo"; None; Some "Foobar"]
        let expected = computeHashFromOptionSeq prime values

        let builder = HashBuilder(prime)
        let result = builder {
            yield! (values |> List.item 2)
            yield! (values |> List.item 1)
            yield! (values |> List.item 0)
        }
        let actual = result.Hash

        test <@ actual = expected @>

    [<Theory>]
    [<InlineDataAttribute(15)>]
    [<InlineDataAttribute(31)>]
    let ``Combine hashcode with mixed values`` (prime) =
        let expected =
            1
            |> combineHash ["Foo"] prime
            |> combineHash [10; 20] prime

        let builder = HashBuilder(prime)
        let result = builder {
            yield 20
            yield 10
            yield "Foo"
        }
        let actual = result.Hash

        test <@ actual = expected @>