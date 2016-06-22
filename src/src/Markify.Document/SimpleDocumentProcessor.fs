namespace Markify.Document

open System

open DocumentHelper
open Markify.Core.Processors
open Markify.Models.Document
open Markify.Models.Definitions

type SimpleDocumentProcessor() =
    interface IDocumentOrganizer with
        member this.Organize (libraries : LibraryDefinition seq, solution, setting : DocumentSetting) : TableOfContent = 
            let pages =
                ([], libraries)
                ||> Seq.fold (fun acc c ->
                    let pageCreator = createPage c.Project setting.PageExtension
                    (acc, c.Types)
                    ||> Seq.fold (fun acc2 d -> (pageCreator d)::acc2))
            let toc = {
                Root = Uri(solution.Path, "docs/")
                Pages = pages }
            toc