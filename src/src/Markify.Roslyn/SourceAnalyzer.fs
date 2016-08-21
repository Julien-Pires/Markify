namespace Markify.Roslyn

open System
open System.IO
open Markify.Core.IO
open Markify.Core.Builders
open Markify.Core.FSharp.Operators
open Markify.Models.Definitions
open Microsoft.CodeAnalysis

type Project = Markify.Models.IDE.Project

type SourceContent = {
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq
}

module SourceAnalyzer =
    let createNamespaceDefinition (node : NamespaceNode) =
        { Name = node.Name }

    let createTypeDefinition ((node : Node), ( typeNode : TypeNode)) = 
        let identity = {
            Name = TypeExtension.getName node
            Parents = TypeExtension.getParentName typeNode
            Namespace = TypeExtension.getNamespaceName node }
        {   Identity = identity
            Kind = typeNode.Kind
            AccessModifiers = typeNode.AccessModifiers
            Modifiers = typeNode.Modifiers
            Parameters = TypeExtension.getGenericParameters typeNode
            BaseTypes = typeNode.Bases }

    let searchDefinitions nodes =
        let content = {
            Namespaces = Seq.empty
            Types = Seq.empty
        }
        nodes
        |> Seq.fold (fun (acc : SourceContent) c ->
            match c with
            | Type x ->
                let typeDefinition = createTypeDefinition (c, x)
                { acc with
                    Types =  seq { yield! acc.Types; yield typeDefinition} }
            | Namespace x ->
                let namespaceDefinition = createNamespaceDefinition x
                { acc with 
                    Namespaces = seq { yield! acc.Namespaces; yield namespaceDefinition } }
            | _ -> acc) content

    let inspect (project : Project) sourceReader =
        let library = {
            Project = project.Name
            Namespaces = Seq.empty
            Types = Seq.empty }
        project.Files
        |> Seq.map (fun c ->
            let nodes = sourceReader c.AbsolutePath
            searchDefinitions nodes )
        |> Seq.fold (fun (acc : LibraryDefinition) c ->
            { acc with
                Namespaces = seq { yield! acc.Namespaces; yield! c.Namespaces}
                Types = seq { yield! acc.Types; yield! c.Types} }) library
        |> fun c -> 
            { c with
                Namespaces = c.Namespaces |> Seq.distinctBy (fun c -> c.Name) 
                Types = c.Types |> Seq.distinctBy (fun c -> c.Identity) }