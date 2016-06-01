namespace Markify.Models

open System

module Document =

    type PageName = string
    type PageFolder = Uri
    type Page = {
        Name : PageName
        Folder : PageFolder
        Content : obj
    }

    type DocumentFolder = Uri
    type PageList = Page seq
    type TableOfContent = {
        Path : DocumentFolder
        Pages : PageList
    }