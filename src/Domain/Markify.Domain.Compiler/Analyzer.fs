namespace Markify.Domain.Compiler

open Markify.Domain.Ide

type IProjectAnalyzer =
    abstract member Analyze: Project -> AssemblyDefinition