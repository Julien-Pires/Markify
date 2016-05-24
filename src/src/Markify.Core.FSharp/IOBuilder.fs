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