namespace Markify.Core.IO

type IOFailure = {
    Message : string
    Stack : string
}

type IOResult<'a> =
    | Success of 'a
    | Failure of IOFailure

type IOBuilder() =
    member this.Bind (x, f) =
        match x with
        | Success a ->
            try
                f a
            with
            | ex -> Failure { Message = ex.Message; Stack = ex.StackTrace }
        | Failure f -> Failure f

    member this.Return x =
        Success x

[<Struct>]
type HashResult (hash : int, hasValue : bool, isOriginal : bool) =
    member this.Hash with get() = hash
    member internal this.HasValue with get() = hasValue
    member internal this.IsOriginal with get() = isOriginal

type HashBuilder(prime : int) =
    let prime = prime
    let identityHash = HashResult(1, true, true)
    let noHash = HashResult(1, false, false)

    let computeHash (a : HashResult) (b : HashResult) =
        HashResult (b.Hash * prime + a.Hash, true, false)

    member this.Zero() =
        noHash
    
    member this.YieldFrom (c : 'a option) =
        match c with
        | Some x -> HashResult(x.GetHashCode(), true, true)
        | None -> noHash
        
    member this.Yield (c) =
        HashResult(c.GetHashCode(), true, true)

    member this.Combine (a : HashResult, b : HashResult) =
        match a.HasValue, b.HasValue with
        | true, false -> a
        | false, true -> b
        | false, false -> noHash
        | _ -> 
            match b.IsOriginal with
            | true -> 
                identityHash
                |> computeHash b 
                |> computeHash a
            | false -> computeHash a b

    member this.Delay (f) =
        f()