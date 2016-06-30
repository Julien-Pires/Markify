namespace Markify.Roslyn

open SyntaxNodeExtension

open Microsoft.CodeAnalysis

[<AbstractClass>]
type LanguageInspector() =
    abstract member getNode : SyntaxNode -> TypeNode option

    member this.Inspect = ignore