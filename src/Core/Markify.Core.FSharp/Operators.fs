namespace Markify.Core.FSharp

type Result<'TData, 'TFailure> =
    | Success of 'TData
    | Failure of 'TFailure

module Operators =
    let bind f input =
        match input with
        | Success x -> f x
        | Failure x -> Failure x

    let (>>=) input f = bind f input