namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn.Common
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open SyntaxHelper

module CSharpParser =
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
        |> Seq.fold (fun (acc : SourceContent) c ->
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
        let tree = CSharpSyntaxTree.ParseText source
        let root = tree.GetRoot()
        getDefinitions root

type CSharpAnalyzer() = 
    interface ILanguageAnalyzer with
        member this.Analyze source = CSharpParser.analyze source

type CSharpSyntax() =
    interface ILanguageSyntax with
        member this.Partial = SyntaxFactory.Token(SyntaxKind.PartialKeyword).Text

[<Language(ProjectLanguage.CSharp)>]
type CSharpModule() =
    interface ILanguageModule with
        member this.Analyzer = CSharpAnalyzer() :> ILanguageAnalyzer
        member this.Syntax = CSharpSyntax() :> ILanguageSyntax