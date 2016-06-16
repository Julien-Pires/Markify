namespace Markify.Document

open Markify.Core.Processors

open Ninject.Modules

type DocumentOrganizerModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<IDocumentOrganizer>().To<SimpleDocumentProcessor>() |> ignore