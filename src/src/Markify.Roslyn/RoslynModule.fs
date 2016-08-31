namespace Markify.Roslyn

open System.Diagnostics.CodeAnalysis
open Markify.Core.Analyzers
open Ninject.Modules

[<ExcludeFromCodeCoverage>]
type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<LanguageConverter>().ToMethod(fun c -> LanguageConverter(CSharpHelper(), ["cs"])) |> ignore
        this.Bind<LanguageConverter>().ToMethod(fun c -> LanguageConverter(VisualBasicHelper(), ["vb"])) |> ignore
        this.Bind<IProjectAnalyzer>().To<RoslynAnalyzer>() |> ignore