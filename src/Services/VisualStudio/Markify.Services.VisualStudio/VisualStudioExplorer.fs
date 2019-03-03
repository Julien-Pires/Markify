namespace Markify.Services.VisualStudio

open System
open System.IO
open Markify.Domain.Ide
open EnvDTE80
open VisualStudioHelper

type VisualStudioExplorer(visualStudio : DTE2) =
    let createProject (project : EnvDTE.Project) = {
        Name = project.Name
        Path = Uri(Path.GetDirectoryName(project.FullName)) 
        Files = getFiles project |> Seq.map Uri
        Language = getLanguage project }

    interface IIDEExplorer with
        member __.Projects 
            with get() =
                visualStudio.Solution
                |> getProjects
                |> Seq.filter (fun c -> c.CodeModel <> null)
                |> Seq.map createProject

        member __.ActiveProject 
            with get() =
                let projects = visualStudio.ActiveSolutionProjects :?> Array
                match projects.Length with
                | 0 -> None
                | _ ->
                    let activeProject = projects.GetValue(0) :?> EnvDTE.Project
                    Some <| createProject activeProject

        member __.ActiveSolution
            with get() =
                match visualStudio.Solution with
                | null -> None
                | c -> Some { Path = Uri(Path.GetDirectoryName(c.FullName)) }