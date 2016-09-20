namespace Markify.Models.Documents

open System

type DocumentFolder = Uri
type FileExtension = string
type DocumentSetting = {
    PageExtension : FileExtension
}

type PageName = string
type PageFolder = Uri
type Page = {
    Name : PageName
    Folder : PageFolder
    Content : obj
}

type TableOfContent = {
    Root : DocumentFolder
    Pages : Page seq
}