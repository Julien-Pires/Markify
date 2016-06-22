namespace Markify.Core.IO

module IO =
    open System.IO

    let private ioBuilder = new IOBuilder()

    let fileExists path =
        ioBuilder {
            let! p = path |> Success
            let! exists = p |> File.Exists |> Success
            return exists }

    let readFile path =
        ioBuilder{
            let! p = path |> Success
            let! content = p |> File.ReadAllText |> Success
            return content }