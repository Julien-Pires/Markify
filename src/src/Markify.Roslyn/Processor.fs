module Processor
    open System

    open Markify.Models
    open Markify.Processors
    open Markify.Models.Context
    open Markify.Models.Definitions

    open Inspectors
    open SourceProvider

    type RoslynProcessor() =
        let inspectFile path =
            let tree = getSyntaxTree path
            match tree with
            | Some s ->
                s.GetRoot() |> searchTypes |> Some
            | None -> None

        let inspectProject (files : FilesList) =
           (Seq.empty<TypeDefinition>, files)
           ||> Seq.fold (fun acc c ->
                let types = inspectFile c.AbsolutePath
                match types with
                | Some s -> Seq.append acc s
                | None -> acc
            )
            |> Seq.distinctBy (fun c -> c.Identity.Fullname)

        interface IProjectProcessor with
            member this.Process (project : ProjectContext) : LibraryDefinition =
                {
                    Types = inspectProject project.Files
                }