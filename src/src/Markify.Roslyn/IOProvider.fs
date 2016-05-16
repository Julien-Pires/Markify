module IOProvider
    open System
    open System.IO
    open System.Security
    open Markify.Roslyn.IO

    type IOErrors =
    | IOError
    | InvalidPath
    | FileDoesNotExists
    | FailedToReadFile
    | FailedToAccessFile

    let ioWorkflow = new IOWorkflow()

    let private fileExists path =
        try
            let exists = File.Exists path
            match exists with
            | true -> Success exists
            | false -> Failure IOErrors.FileDoesNotExists
        with
        | :? ArgumentException
        | :? ArgumentNullException -> Failure IOErrors.InvalidPath
        | _ -> Failure IOErrors.IOError

    let private readText path =
        try
            Success (File.ReadAllText path)
        with
        | :? ArgumentException
        | :? ArgumentNullException -> Failure IOErrors.InvalidPath
        | :? FileNotFoundException -> Failure IOErrors.FileDoesNotExists
        | :? SecurityException
        | :? UnauthorizedAccessException -> Failure IOErrors.FailedToAccessFile
        | _ -> Failure IOErrors.FailedToReadFile

    let readFile path =
        ioWorkflow{
            let! exists = fileExists path
            let! content = readText path
            return content
        }