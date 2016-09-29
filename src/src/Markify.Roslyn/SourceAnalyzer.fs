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

    let createTypeDefinition ((node : Node), (typeNode : TypeNode)) : TypeDefinition option = 
        let identity = {
            Name = TypeExtension.getName node
            Parents = TypeExtension.getParentName typeNode
            Namespace = TypeExtension.getNamespaceName node
            AccessModifiers = typeNode.AccessModifiers
            Modifiers = typeNode.Modifiers
            Parameters = TypeExtension.getGenericParameters typeNode
            BaseTypes = typeNode.Bases }
        match typeNode.Kind with
        | StructureKind.Class -> Some <| Class { Identity = identity }
        | StructureKind.Struct -> Some <| Struct { Identity = identity }
        | StructureKind.Interface -> Some <| Interface { Identity = identity }
        | StructureKind.Enum -> Some <| Enum { Identity = identity }
        | StructureKind.Delegate -> Some <|  Delegate { Identity = identity }
        | _ -> None

    let searchDefinitions nodes =
        ({ Namespaces = []; Types = [] }, nodes)
        ||> List.fold (fun acc c ->
            match c with
            | Type x ->
                let typeDefinition = createTypeDefinition (c, x)
                match typeDefinition with
                | Some w -> { acc with Types =  w::acc.Types }
                | _ -> acc
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