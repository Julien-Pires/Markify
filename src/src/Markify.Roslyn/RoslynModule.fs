namespace Markify.Roslyn

open Markify.Core.Processors

open Ninject.Modules

type RoslynModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<IProjectAnalyzer>().To<RoslynProcessor>() |> ignore