namespace Markify.Services.Document

open System
open System.IO
open Markify.Domain.Document
open Markify.Domain.Compiler

module DocumentHelper =
    let convertIdentityToPath identity =
        let mergedName =
            match (identity.Namespace, identity.Parents) with
            | Some x, Some w -> sprintf "%s.%s" x w
            | Some x, None -> x
            | None, Some x -> x
            | _ -> ""
        match mergedName with
        | "" -> ""
        | _ -> sprintf @"%s\" <| mergedName.Replace ('.', '\\')

    let cleanExtension (ext : string) = 
        match ext with
        | ext when ext.StartsWith(".") -> ext.Substring(1)
        | _ -> ext

    let createPage project ext (definition : TypeDefinition) =
        let path = Path.Combine (project, convertIdentityToPath definition.Identity)
        let cleanedExt = cleanExtension ext
        {   Name = sprintf "%s.%s" definition.Identity.Name cleanedExt
            Folder = Uri (path, UriKind.Relative)
            Content = definition }