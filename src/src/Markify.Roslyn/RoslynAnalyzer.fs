namespace Markify.Roslyn

open Markify.Core.Analyzers
open Markify.Models.Definitions

type RoslynAnalyzer() =
    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : LibraryDefinition =
            SourceAnalyzer.inspect project