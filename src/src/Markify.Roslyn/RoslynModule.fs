namespace Markify.Roslyn

open Markify.Core.Analyzers
open Ninject.Modules

type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<IProjectAnalyzer>().To<RoslynAnalyzer>() |> ignore