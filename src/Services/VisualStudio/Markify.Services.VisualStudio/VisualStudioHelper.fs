namespace Markify.Services.VisualStudio

open EnvDTE
open EnvDTE80
open Markify.Domain.Ide

module VisualStudioHelper =
    let getProjects (solution : EnvDTE.Solution) =
        let rec loop acc (item : EnvDTE.Project) =
            match item.Kind with
            | x when x = ProjectKinds.vsProjectKindSolutionFolder ->
                item.ProjectItems
                |> Seq.cast<EnvDTE.ProjectItem>
                |> Seq.choose (fun c -> match c.SubProject with | null -> None | x -> Some x)
                |> Seq.fold loop acc
            | _ -> item::acc

        solution.Projects
        |> Seq.cast<EnvDTE.Project>
        |> Seq.fold loop []

    let getFiles (project : EnvDTE.Project) =
        let rec loop acc (item : EnvDTE.ProjectItem) =
            match item.Kind with
            | x when
                x = Constants.vsProjectItemKindPhysicalFolder ||
                x = Constants.vsProjectItemKindVirtualFolder ->
                item.ProjectItems
                |> Seq.cast<EnvDTE.ProjectItem>
                |> Seq.fold loop acc
            | _ -> item.FileNames(0s)::acc

        project.ProjectItems
        |> Seq.cast<EnvDTE.ProjectItem>
        |> Seq.fold loop []

    let getLanguage (project : EnvDTE.Project) = 
        match project.CodeModel with
        | null -> ProjectLanguage.Unsupported
        | x ->
            match x.Language with
            | CodeModelLanguageConstants.vsCMLanguageCSharp -> ProjectLanguage.CSharp
            | CodeModelLanguageConstants.vsCMLanguageVB -> ProjectLanguage.VisualBasic
            | _ -> ProjectLanguage.Unsupported