namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Models.Definitions
open Microsoft.CodeAnalysis

type Project = Markify.Models.IDE.Project

module SourceAnalyzer =
    let (|HasExtension|_|) (uri : string) expected = 
        let result = uri.EndsWith (expected, StringComparison.CurrentCultureIgnoreCase)
        match result with
        | true -> Some true
        | _ -> None

    let getNodeFactory file =
        let ext = Path.GetExtension file
        let languageHelper =
            match ext with
            | HasExtension ".cs" _ -> Some (CSharpHelper() :> NodeHelper)
            | HasExtension ".vb" _ -> Some (VisuaBasicHelper() :> NodeHelper)
            | _ -> None
        let factory =
            match languageHelper with
            | Some x -> Some (NodeFactory(x))
            | _ -> None
        factory

    let readFile file =
        match IO.readFile file with
        | Success x -> Some x
        | _ -> None

    let searchTypes nodes =
        nodes
        |> Seq.choose (fun c ->
            match c with
            | Type x ->
                let identity = {
                    Name = TypeExtension.getName c 
                    Parents = Seq.empty
                    Namespace = None }
                let typeObj = {
                    Identity = identity
                    Kind = x.Kind
                    AccessModifiers = x.AccessModifiers
                    Modifiers = x.Modifiers
                    Parameters = TypeExtension.getGenericParameters x
                    BaseTypes = x.Bases }
                Some typeObj
            | _ -> None)

    let (>>=) m f = Option.bind f m
    let getDefinitions file =
        let nodes =
            readFile file
            >>= (fun c ->
            getNodeFactory file 
            >>= (fun d -> Some (d.GetNodes c)))
        match nodes with
        | Some x -> searchTypes x
        | None -> Seq.empty

    let inspect (project : Project) =
        let types = 
            project.Files
            |> Seq.fold (fun acc c ->
                getDefinitions c.AbsolutePath
                |> Seq.append acc
                |> Seq.distinctBy (fun d -> d.Identity.Name)) Seq.empty
        let lib = {
            Project = project.Name
            Namespaces = Seq.empty
            Types = types }
        lib