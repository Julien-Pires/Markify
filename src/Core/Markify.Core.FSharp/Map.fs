namespace Markify.Core.FSharp

module MapBuilderModule =
    let combine x (y : Map<_,_>) = y |> Seq.fold (fun acc c -> acc |> Map.add c.Key c.Value) x

type MapBuilder() =
    member this.Bind(m, f) = f m
    member this.Zero() = Map.empty
    member this.Yield(x) = [x] |> Map.ofList
    member this.YieldFrom(x) = x
    member this.Combine(x, y) = MapBuilderModule.combine x y
    member this.Delay(f) = f()
    member this.For(m, f) = m |> Seq.map f |> Seq.fold MapBuilderModule.combine Map.empty