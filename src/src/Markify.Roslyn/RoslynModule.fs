namespace Markify.Roslyn

open System.Diagnostics.CodeAnalysis
open Markify.Core.Analyzers
open Ninject.Modules

[<ExcludeFromCodeCoverage>]
type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        let analyzers = seq {
            yield CSharpAnalyzer.analyze, seq { yield "cs" }
            yield VisualBasicAnalyzer.analyze, seq { yield "vb" } }

        this.Bind<IProjectAnalyzer>().To<RoslynAnalyzer>().WithConstructorArgument(analyzers) |> ignore