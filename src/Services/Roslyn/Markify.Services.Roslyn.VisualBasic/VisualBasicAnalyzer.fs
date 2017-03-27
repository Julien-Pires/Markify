namespace Markify.Services.Roslyn.VisualBasic

open VisualBasicSyntaxHelper
open Markify.Domain.Ide
open Markify.Services.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open SyntaxHelper

module VisualBasicParser =
    let isDefinitionNode = function
        | IsStructureType x -> Some (x :> SyntaxNode)
        | IsNamespace x -> Some (x :> SyntaxNode)
        | IsEnum x -> Some (x :> SyntaxNode)
        | IsDelegate x -> Some (x :> SyntaxNode)
        | _ -> None

    let isVisitableNode = function
        | IsStructureType x -> Some (x :> SyntaxNode)
        | IsNamespace x -> Some (x :> SyntaxNode)
        | _ -> None

    let getDefinitions (root : SyntaxNode) =
        findDefinitionNodes isVisitableNode isDefinitionNode root
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

type VisualBasicAnalyzer() = 
    interface ILanguageAnalyzer with
        member this.Analyze source = VisualBasicParser.analyze source

type VisualBasicSyntax() =
    interface ILanguageSyntax with
        member this.Partial = SyntaxFactory.Token(SyntaxKind.PartialKeyword).Text

[<Language(ProjectLanguage.VisualBasic)>]
type VisualBasicModule() =
    interface ILanguageModule with
        member this.Analyzer = VisualBasicAnalyzer() :> ILanguageAnalyzer
        member this.Syntax = VisualBasicSyntax() :> ILanguageSyntax