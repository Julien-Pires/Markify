namespace Markify.Document

open System
open System.IO

open Markify.Models.Documents
open Markify.Models.Definitions

module DocumentHelper =
    let convertNameToPath (fullname : string) =
        let parts = fullname.Split ('.')
        let path = String.Join ("\\", parts, 0, parts.Length - 1)
        sprintf @"%s\" path

    let cleanExtension (ext : string) = 
        match ext with
        | ext when ext.StartsWith(".") -> ext.Substring(1)
        | _ -> ext

    let createPage project ext definition =
        let path = Path.Combine (project, convertNameToPath definition.Identity.Fullname)
        let cleanExt = cleanExtension ext
        let page = {
            Name = sprintf "%s.%s" definition.Identity.Name cleanExt
            Folder = Uri (path, UriKind.Relative)
            Content = definition }
        page