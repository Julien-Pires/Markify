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

type ProjectAnalyzer(analyzers : ISourceAnalyzer seq) =
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
        member __.Analyze project = {
            Project = project.Name
            Namespaces = [] 
            Types = [] }