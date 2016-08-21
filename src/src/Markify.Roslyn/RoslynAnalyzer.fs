namespace Markify.Roslyn

open Markify.Core.Analyzers
open Markify.Models.Definitions

type RoslynAnalyzer (sourceReader : SourceReader) =
    let sourceReader = sourceReader
    
    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : LibraryDefinition =
            SourceAnalyzer.inspect project sourceReader.GetNodes