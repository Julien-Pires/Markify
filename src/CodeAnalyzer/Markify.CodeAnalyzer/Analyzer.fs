namespace Markify.CodeAnalyzer

type Project = {
    Name : string 
    Content : IProjectContent seq }

type Assemblyinfo = {
    Project : Name
    Namespaces : NamespaceInfo seq
    Types : Definition seq }

type IProjectAnalyzer =
    abstract member Analyze: Project -> Assemblyinfo

type ProjectAnalyzer(analyzers: ISourceAnalyzer seq) =
    let sourceAnalyzers =
        analyzers
        |> Seq.choose (fun c ->
            let typeInfo = c.GetType()
            let attribute = typeInfo.GetCustomAttributes(typeof<LanguageAttribute>, false)
            match attribute |> Seq.tryHead with
            | Some x -> Some ((x :?> LanguageAttribute).Language, c)
            | None -> None)
        |> Map.ofSeq

    interface IProjectAnalyzer with
        member __.Analyze project =
            let results = seq {
                for content in project.Content do
                    let analyzer = sourceAnalyzers |> Map.find content.Language
                    let { Namespaces = _; Definitions = ds} = analyzer.Analyze content 
                    yield! ds |> Seq.map (fun c -> c.Definition) }
            { Project = project.Name
              Namespaces = [] 
              Types = results }