namespace Markify.Roslyn.IO

type IOAction<'a, 'b> =
    | Success of 'a
    | Failure of 'b

type IOWorkflow() =
    member this.Bind (x, f) =
        match x with
        | Success a -> f a
        | Failure f -> Failure f

    member this.Return x =
        Success x