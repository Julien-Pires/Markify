namespace Markify.CodeAnalyzer

type ProjectLanguage =
    | Unknown = 0
    | CSharp = 1
    | VisualBasic = 2

type IProjectContent =
    abstract member Content : string with get
    abstract member Language : ProjectLanguage with get

type Project = {
    Name : string 
    Content : IProjectContent seq }

type IProjectAnalyzer =
    abstract member Analyze: Project -> AssemblyDefinition