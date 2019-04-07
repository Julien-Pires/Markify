namespace Markify.Services.Document

open System
open DocumentHelper
open Markify.Domain.Document
open Markify.CodeAnalyzer

type SimpleDocumentOrganizer() =
    interface IDocumentOrganizer with
        member this.Organize libraries root setting = 
            let pages =
                ([], libraries)
                ||> Seq.fold (fun acc c ->
                    let pageCreator = createPage c.Project setting.PageExtension
                    (acc, c.Types)
                    ||> Seq.fold (fun acc2 d -> (pageCreator d)::acc2))
            {   Root = Uri(root, "docs/")
                Pages = pages }