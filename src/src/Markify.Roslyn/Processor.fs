module Processor
    open Markify.Models
    open Markify.Processors

    type RoslynProcessor() =
        interface IProjectProcessor with
            member this.Process (project : ProjectContext) : LibraryDefinition = null