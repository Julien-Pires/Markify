module SourceProvider
    open Markify.Roslyn.IO
    open Microsoft.CodeAnalysis.CSharp
    
    let getSyntaxTree path =
        match IOProvider.readFile path with
        | Success a -> Some (CSharpSyntaxTree.ParseText a)
        | Failure f -> None