namespace Markify.CodeAnalyzer

open System

type ProjectLanguage = string

type IProjectContent =
    abstract member Content : string with get
    abstract member Language : ProjectLanguage with get

type DefinitionResult = {
    Definition : Definition 
    IsPartial : bool }

type AnalyzeResult = {
    Namespaces : NamespaceInfo seq 
    Definitions : DefinitionResult seq }

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)>]
type LanguageAttribute(language) =
    inherit Attribute()
    member __.Language : ProjectLanguage = language

type ISourceAnalyzer =
    abstract member Analyze: IProjectContent -> AnalyzeResult