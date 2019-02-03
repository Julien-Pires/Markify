namespace Markify.Domain.Compiler

type ProjectLanguage =
    | Unknown
    | CSharp
    | VisualBasic

type IProjectContent =
    abstract member Content : string with get
    abstract member Language : ProjectLanguage with get

type Project = {
    Name : string 
    Content : IProjectContent list }

type IProjectAnalyzer =
    abstract member Analyze: Project -> AssemblyDefinition