namespace Markify.Document

open System

open Markify.Core.Processors
open Markify.Models.Document
open Markify.Models.Definitions

module Processor =
    type DocumentProcessor() =
        interface IDocumentProcessor with
            member this.Process (library : LibraryDefinition) : TableOfContent = 
                {
                    Path = new Uri("");
                    Pages = Seq.empty<Page>
                }