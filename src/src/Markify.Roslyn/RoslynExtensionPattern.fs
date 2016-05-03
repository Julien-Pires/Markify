module RoslynExtensionPattern
    open Microsoft.CodeAnalysis

    let (|ClassNode|_|) (node: SyntaxNode) = None

    let (|DelegateNode|_|) (node: SyntaxNode) = None

    let (|StructureNode|_|) (node: SyntaxNode) =
        match node with
        | ClassNode -> None
        | DelegateNode -> None
        | _ -> None