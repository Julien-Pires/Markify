namespace Markify.Roslyn

open SyntaxNodeExtension

open Microsoft.CodeAnalysis

[<AbstractClass>]
type LanguageInspector() =
    abstract member getNode : SyntaxNode -> Node

    member this.Inspect = ignore