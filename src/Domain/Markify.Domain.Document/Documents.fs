namespace Markify.Domain.Document

open System

type PageName = string
type Folder = Uri
type Page = {
    Name : PageName
    Folder : Folder
    Content : obj }

type TableOfContent = {
    Root : Folder
    Pages : Page seq }