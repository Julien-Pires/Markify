namespace Markify.Roslyn

open System.Diagnostics.CodeAnalysis
open Markify.Core.Analyzers
open Ninject.Modules

[<ExcludeFromCodeCoverage>]
type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        let languageHelpers = [
            CSharpHelper() :> NodeHelper, ["cs"]
            VisualBasicHelper() :> NodeHelper, ["vb"]
        ]

        this.Bind<SourceConverter>().ToSelf().WithConstructorArgument(languageHelpers) |> ignore
        this.Bind<IProjectAnalyzer>().To<RoslynAnalyzer>() |> ignore