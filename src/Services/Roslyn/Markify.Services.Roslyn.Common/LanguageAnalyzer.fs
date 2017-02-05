namespace Markify.Services.Roslyn.Common

type ILanguageAnalyzer =
    abstract member Extensions: string seq with get

    abstract member Analyze: string -> SourceContent