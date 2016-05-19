module SourceProvider
    open Markify.Core.IO
    open Microsoft.CodeAnalysis.CSharp
    
    let getSyntaxTree path =
        match IO.readFile path with
        | Success a -> Some (CSharpSyntaxTree.ParseText a)
        | Failure f -> None