namespace Markify.Domain.Rendering

open System
open Markify.Domain.Document

type IPageWriter =
    abstract member Write: string -> Page -> Uri -> unit

type IDocumentRenderer =
    abstract member Render: TableOfContent -> IPageWriter -> unit