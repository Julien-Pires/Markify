namespace Markify.Services.Roslyn.Common

open System
open Markify.Domain.Ide

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)>]
type LanguageAttribute(language) =
    member this.Language : ProjectLanguage = language

type ILanguageAnalyzer =
    abstract member Analyze: string -> SourceContent

type ILanguageSyntax =
    abstract member Partial: string with get

type ILanguageModule =
    abstract member Analyzer: ILanguageAnalyzer with get
    abstract member Syntax: ILanguageSyntax with get