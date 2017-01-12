namespace Markify.Services.Roslyn.VisualBasic

open VisualBasicSyntaxHelper
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic

module VisualBasicAnalyzer =
    let getDefinitions (root : SyntaxNode) =
        root.DescendantNodes()
        |> Seq.fold (fun (acc :SourceContent) c ->
            match c with
            | IsStructureType x ->
                let structureDefinition = StructureDefinitionFactory.create x
                { acc with Types = structureDefinition::acc.Types }
            | IsEnum x ->
                let enumDefinition = EnumDefinitionFactory.create x
                { acc with Types = enumDefinition::acc.Types }
            | IsDelegate x ->
                let delegateDefinition = DelegateDefinitionFactory.create x
                { acc with Types = delegateDefinition::acc.Types }
            | IsNamespace x -> 
                let namespaceDefinition = NamespaceDefinitionFactory.create x
                { acc with Namespaces = namespaceDefinition::acc.Namespaces }
            | _ -> acc) { Types = []; Namespaces = [] }

    let analyze (source : string) =
        let tree = VisualBasicSyntaxTree.ParseText source
        let root = tree.GetRoot()
        getDefinitions root
