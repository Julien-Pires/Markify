namespace Markify.Document

open System.Diagnostics.CodeAnalysis
open Markify.Core.Analyzers

open Ninject.Modules

[<ExcludeFromCodeCoverage>]
type DocumentationOrganizerModule() =
    inherit NinjectModule()

    override this.Load () =
        this.Bind<IDocumentationOrganizer>().To<BasicDocumentationOrganizer>() |> ignore