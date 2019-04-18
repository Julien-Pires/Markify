﻿namespace Markify.CodeAnalyzer.Roslyn.Common

open System
open Markify.CodeAnalyzer

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)>]
type LanguageAttribute(language) =
    inherit Attribute()
    member __.Language : ProjectLanguage = language

type ILanguageAnalyzer =
    abstract member Analyze: string -> SourceContent

type ILanguageSyntax =
    abstract member Partial: string with get

type ILanguageModule =
    abstract member Analyzer: ILanguageAnalyzer with get
    abstract member Syntax: ILanguageSyntax with get