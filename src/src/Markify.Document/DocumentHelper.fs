namespace Markify.Document

open System
open System.IO

open Markify.Models.Document
open Markify.Models.Definitions

module DocumentHelper =
    let convertNameToPath (fullname : string) =
        fullname.Replace ('.', '\\')

    let cleanExtension (ext : string) = 
        match ext with
        | ext when ext.StartsWith(".") -> ext.Substring(1)
        | _ -> ext

    let createPage project ext definition =
        let path = Path.Combine (project, convertNameToPath definition.Identity.Fullname)
        let cleanExt = cleanExtension ext
        let fullPath = sprintf "%s.%s" path cleanExt
        let page = {
            Name = sprintf "%s.%s" definition.Identity.Name cleanExt
            Folder = Uri (fullPath, UriKind.Relative)
            Content = definition
        }
        page