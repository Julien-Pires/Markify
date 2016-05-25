module Processor
    open Markify.Models
    open Markify.Processors
    open Markify.Models.Context
    open Markify.Models.Definitions

    open Inspectors
    open SourceProvider
    open SyntaxNodeExtension

    type RoslynProcessor() =
        let structureSearches = [
            searchTypes (|ClassNode|_|)
            searchTypes (|InterfaceNode|_|)
            searchTypes (|StructNode|_|)
        ]

        let inspectFile path =
            let tree = getSyntaxTree path
            match tree with
            | Some s ->
                structureSearches 
                |> Seq.collect (fun c -> c (s.GetRoot()))
                |> Some
            | None -> None

        let inspectProject (files : FilesList) =
           (Seq.empty, files)
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