namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Models.Definitions
open Markify.Roslyn.LanguageHelper
open Microsoft.CodeAnalysis

type Project = Markify.Models.IDE.Project

module SourceAnalyzer =
    let (|HasExtension|_|) (uri : string) expected = 
        let result = uri.EndsWith (expected, StringComparison.CurrentCultureIgnoreCase)
        match result with
        | true -> Some true
        | _ -> None

    let getLanguageAnalyzer file =
        let ext = Path.GetExtension file
        match ext with
        | HasExtension ".cs" x -> Some CSharpModule.inspect
        | _ -> None

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
                    Fullname = TypeExtension.getFullname c
                    Name = TypeExtension.getName c }
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
            getLanguageAnalyzer file 
            >>= (fun d -> Some (d c)))
        match nodes with
        | Some x -> searchTypes x
        | None -> Seq.empty

    let inspect (project : Project) =
        let types = 
            project.Files
            |> Seq.fold (fun acc c ->
                getDefinitions c.AbsolutePath
                |> Seq.append acc
                |> Seq.distinctBy (fun d -> d.Identity.Fullname)) Seq.empty
        let lib = {
            Project = project.Name
            Types = types }
        lib