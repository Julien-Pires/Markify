namespace Markify.Roslyn

open SyntaxNodeExtension

open Microsoft.CodeAnalysis

[<AbstractClass>]
type LanguageInspector() =
    member this.Inspect : Node seq = Seq.empty