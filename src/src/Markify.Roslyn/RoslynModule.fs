namespace Markify.Roslyn

open Markify.Core.Analyzers
open Ninject.Modules

type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<LanguageReader>().ToMethod(fun c -> LanguageReader(CSharpHelper(), ["cs"])) |> ignore
        this.Bind<LanguageReader>().ToMethod(fun c -> LanguageReader(VisualBasicHelper(), ["vb"])) |> ignore
        this.Bind<IProjectAnalyzer>().To<RoslynAnalyzer>() |> ignore