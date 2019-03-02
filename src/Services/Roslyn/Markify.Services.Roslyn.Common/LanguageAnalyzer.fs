namespace Markify.Services.Roslyn.Common

open System
open Markify.Domain.Compiler

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)>]
type LanguageAttribute(language) =
    member __.Language : ProjectLanguage = language

type ILanguageAnalyzer =
    abstract member Analyze: string -> SourceContent

type ILanguageSyntax =
    abstract member Partial: string with get

type ILanguageModule =
    abstract member Analyzer: ILanguageAnalyzer with get
    abstract member Syntax: ILanguageSyntax with get