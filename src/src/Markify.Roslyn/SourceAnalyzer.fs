namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Core.Builders
open Markify.Core.FSharp.Operators
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

    let searchDefinitions nodes =
        nodes
        |> Seq.fold (fun acc c ->
            let namespaces, types = acc
            match c with
            | Type x ->
                let identity = {
                    Name = TypeExtension.getName c 
                    Parents = TypeExtension.getParentName x
                    Namespace = TypeExtension.getNamespaceName c }
                let typeDef = {
                    Identity = identity
                    Kind = x.Kind
                    AccessModifiers = x.AccessModifiers
                    Modifiers = x.Modifiers
                    Parameters = TypeExtension.getGenericParameters x
                    BaseTypes = x.Bases }
                (namespaces, seq { yield! types; yield typeDef })
            | Namespace x ->
                let namespaceDef = {
                    Name = x.Name}
                (seq { yield! namespaces; yield namespaceDef}, types)
            | _ -> acc) (Seq.empty, Seq.empty)

    let analyzeSourceFile file =
        let nodes =
            readFile file
            >>= (fun c ->
            getNodeFactory file 
            >>= (fun d -> Some (d.GetNodes c)))
        match nodes with
        | Some x -> searchDefinitions x
        | None -> (Seq.empty, Seq.empty)

    let inspect (project : Project) =
        let namespaces, types = 
            project.Files
            |> Seq.fold (fun acc c ->
                let v, w = acc
                let n, t = analyzeSourceFile c.AbsolutePath
                (seq { yield! v; yield! n }, seq { yield! w; yield! t })) (Seq.empty, Seq.empty)
        let library = {
            Project = project.Name
            Namespaces =
                namespaces
                |> Seq.distinctBy (fun c -> c.Name)
                |> Seq.toList
            Types = 
                types
                |> Seq.distinctBy (fun c -> c.Identity)
                |> Seq.toList }
        library