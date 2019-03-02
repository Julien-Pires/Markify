namespace Markify.Tests.Extension

open Expecto

[<AutoOpen>]
module Expecto =
    let testRepeat repeater name test =
        repeater test
        |> Seq.mapi (fun index c -> testCase (sprintf "%s #%i" name index) c)

    let testRepeatParameterized name setup test =
        setup
        |> Seq.collect (fun (repeater, parameters) ->
            repeater test |> Seq.mapi (fun index c -> testCase (sprintf "%s (%A) #%i" name parameters index) (c parameters)))