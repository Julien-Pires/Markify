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
    Namespaces : NamespaceDefinition list
    Types : TypeDefinition list
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
        ({ Namespaces = []; Types = [] }, nodes)
        ||> List.fold (fun acc c ->
            match c with
            | Type x ->
                let typeDefinition = createTypeDefinition (c, x)
                { acc with Types =  typeDefinition::acc.Types }
            | Namespace x ->
                let namespaceDefinition = createNamespaceDefinition x
                { acc with Namespaces = namespaceDefinition::acc.Namespaces }
            | _ -> acc)

    let inspect (project : Project) (sourceConverter : SourceConverter) =
        let definitions =
            ({ Namespaces = []; Types = [] }, project.Files)
            ||> Seq.fold (fun acc c ->
                let nodes = sourceConverter.Convert c.AbsolutePath
                let content = searchDefinitions nodes
                { acc with
                    Namespaces = List.append acc.Namespaces content.Namespaces
                    Types = List.append acc.Types content.Types })
            |> fun c ->
                { c with
                    Namespaces = c.Namespaces |> List.distinctBy (fun c -> c.Name) 
                    Types = c.Types |> List.distinctBy (fun c -> c.Identity) }
        {   Project = project.Name
            Namespaces = definitions.Namespaces
            Types = definitions.Types }