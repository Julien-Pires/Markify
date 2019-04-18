namespace Markify.Domain.Document

open Markify.CodeAnalyzer

type IDocumentOrganizer =
    abstract member Organize: Assemblyinfo seq -> Folder -> DocumentSetting -> TableOfContent