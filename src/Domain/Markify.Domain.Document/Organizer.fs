namespace Markify.Domain.Document

open Markify.Domain.Compiler

type IDocumentOrganizer =
    abstract member Organize: AssemblyDefinition seq -> Folder -> DocumentSetting -> TableOfContent