module Processor
    open Markify.Models
    open Markify.Processors
    open Markify.Models.Context
    open Markify.Models.Definitions

    type RoslynProcessor() =
        interface IProjectProcessor with
            member this.Process (project : ProjectContext) : LibraryDefinition =
                { 
                    Types = Seq.empty<TypeDefinition> 
                }