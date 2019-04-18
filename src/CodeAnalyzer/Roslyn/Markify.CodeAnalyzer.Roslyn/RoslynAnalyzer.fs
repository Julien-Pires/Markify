namespace Markify.CodeAnalyzer.Roslyn

open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn.Common
open Markify.Core.FSharp

type RoslynAnalyzer (languages : ILanguageModule seq) =
    let getLanguage languageModule =
        let languageType = languageModule.GetType()
        let attr = languageType.GetCustomAttributes(typeof<LanguageAttribute>, false)   
        match attr |> Seq.tryHead with
        | None -> ProjectLanguage.Unknown
        | Some x -> (x :?> LanguageAttribute).Language

    let languageModules =
        languages
        |> Seq.fold (fun acc c ->
            match getLanguage c with
            | ProjectLanguage.Unknown -> acc
            | x -> (x, c)::acc) []
        |> Map.ofList

    let merge types newTypes languageSyntax = MapBuilder(){
        yield! MissingTypeMerger.merge types newTypes
        yield! PartialMerger.merge types newTypes languageSyntax
        yield! types
    }

    let analyzeSources (content : IProjectContent seq) =
        let types, namespaces =            
            Seq.fold (fun acc (c : IProjectContent) ->              
                let languageModule = languageModules.TryFind c.Language
                match languageModule with
                | Some x ->
                    let types, namespaces = acc
                    let content = c.Content |> x.Analyzer.Analyze
                    (   merge types content.Types x.Syntax,
                        content.Namespaces |> List.append namespaces)
                | None -> acc) (Map.empty, []) content
        (   types |> Seq.map (fun d -> d.Value) |> Seq.toList,
            namespaces |> List.distinct )    

    interface IProjectAnalyzer with
        member __.Analyze (project : Project) : Assemblyinfo =
            let types, namespaces = analyzeSources project.Content
            {   Project = project.Name
                Namespaces = namespaces
                Types = types }