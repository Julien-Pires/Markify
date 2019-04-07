namespace Markify.Domain.Document

open Markify.CodeAnalyzer

type IDocumentOrganizer =
    abstract member Organize: AssemblyDefinition seq -> Folder -> DocumentSetting -> TableOfContent