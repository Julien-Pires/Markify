namespace Markify.Roslyn

open Markify.Models.IDE
open Markify.Models.Definitions

open Markify.Core.Analyzers

type RoslynAnalyzer() =
    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : LibraryDefinition =
            SourceAnalyzer.inspect project