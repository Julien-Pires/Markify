namespace Markify.Core.IO

type IOException = {
    Message : string
    Stack : string
}

type IOFailure =
    | Error of string
    | Exception of IOException

type IOAction<'a> =
    | Success of 'a
    | Failure of IOFailure

type IOWorkflow() =
    member this.Bind (x, f) =
        match x with
        | Success a -> f a
        | Failure f -> Failure f

    member this.Return x =
        Success x