namespace Markify.Document

open Markify.Core.Analyzers

open Ninject.Modules

type DocumentationOrganizerModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<IDocumentationOrganizer>().To<SimpleDocumentationOrganizer>() |> ignore