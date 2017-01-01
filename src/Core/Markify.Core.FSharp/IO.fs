namespace Markify.Core.FSharp.IO

module IO =
    open System.IO
    open Markify.Core.FSharp.Builders

    let private tryBuilder = new TryBuilder()

    let fileExists path =
        tryBuilder {
            let! p = path |> Success
            let! exists = p |> File.Exists |> Success
            return exists }

    let readFile path =
        tryBuilder{
            let! p = path |> Success
            let! content = p |> File.ReadAllText |> Success
            return content }