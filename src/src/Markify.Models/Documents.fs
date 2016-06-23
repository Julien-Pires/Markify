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
    
type PageList = Page seq
type TableOfContent = {
    Root : DocumentFolder
    Pages : PageList
}