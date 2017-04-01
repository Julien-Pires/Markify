namespace Markify.Core.FSharp.Tests

open Markify.Core.FSharp
open Swensen.Unquote
open Xunit
open Ploeh.AutoFixture.Xunit2

module Map_Tests =
    
    [<Fact>]
    let ``MapBuilder should return empty map when there is no instruction``() =
        let actual = MapBuilder(){ printf "Foo" }

        test <@ Map.isEmpty actual @>

    [<Fact>]
    let ``MapBuilder should return map with one value when calling yield once``() =
        let actual = MapBuilder() { yield ("Foo", "Bar") }

        test <@ Seq.length actual = 1 @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should add key and value to map when calling yield once``(value : string * string) =
        let actual = MapBuilder() { yield value }

        test <@ Map.find (fst value) actual = snd value @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should return same map when calling yield! once``(value : string * string) =
        let expected = [value] |> Map.ofList
        let actual = MapBuilder() { yield! expected }

        test <@ actual = expected @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should return complete map when calling yield multiple times``(valueOne : string * string, valueTwo : string * string) =
        let expected = [valueOne; valueTwo] |> Map.ofList
        let actual = MapBuilder(){
            yield valueOne
            yield valueTwo
        }

        test <@ actual = expected @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should return complete map when calling yield! multiple times``(valuesOne : (string * string) seq, valuesTwo : (string * string) seq) =
        let expected = Seq.append valuesOne valuesTwo |> Map.ofSeq
        let actual = MapBuilder(){
            yield! valuesOne |> Map.ofSeq
            yield! valuesTwo |> Map.ofSeq
        }

        test <@ actual = expected @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should return complete map when calling yield multiple times in for loop``(values : (string * string) seq) =
        let expected = values |> Map.ofSeq
        let actual = MapBuilder() {
            for t in values do
                yield t
        }

        test <@ actual = expected @>

    [<Theory>]
    [<AutoData>]
    let ``MapBuilder should return complete map when calling yield! multiple times in for loop``(maps : Map<string, string> seq) =
        let expected = maps |> Seq.collect (fun c -> c |> Map.toSeq) |> Map.ofSeq
        let actual = MapBuilder() {
            for c in maps do
                yield! c
        }

        test <@ actual = expected @>