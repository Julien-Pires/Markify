module Customization

    open System
    open System.Collections.Generic

    open Markify.Document
    open Markify.Core.Analyzers

    open Markify.Models.IDE
    open Markify.Models.Documents
    open Markify.Models.Definitions

    open Ploeh.AutoFixture

    type SimpleDocumentCustomization (root, projectName, projectCount, typeCount, extension) =
        let root = root
        let projectName = projectName;
        let projectCount = projectCount
        let typeCount = typeCount

        interface ICustomization with
            member  this.Customize (fixture : IFixture) = 
                fixture.Inject<IDocumentationOrganizer> (new SimpleDocumentationOrganizer())

                fixture.Register<Solution>(fun c ->
                    let solution = {
                        Name = "Solution"
                        Path = Uri(root)
                        Projects = [] }
                    solution)
                fixture.Register<DocumentSetting> (fun c -> 
                    let setting = {
                        PageExtension = extension }
                    setting)
                fixture.Register<IEnumerable<LibraryDefinition>> (fun c ->
                    match projectCount with
                    | 0 -> Seq.empty
                    | _ ->
                        seq { 0..(projectCount - 1) }
                        |> Seq.map (fun c -> this.CreateLibrary (sprintf "%s%i" projectName c) typeCount))

        member this.CreateLibrary name typeCount = 
            let types =
                match typeCount with
                | 0 -> Seq.empty
                | _ ->
                    seq { 0..(typeCount - 1) }
                    |> Seq.map (fun c ->
                        let identity = {
                            Name = sprintf "Type%i" c;
                            Parents = Some "Parent"
                            Namespace = None }
                        let typeDef = {
                            Identity = identity
                            Kind = StructureKind.Class
                            AccessModifiers = Seq.empty
                            Modifiers = Seq.empty
                            BaseTypes = Seq.empty
                            Parameters = Seq.empty }
                        typeDef)
            let lib = {
                Project = name
                Namespaces = Seq.empty
                Types = types }
            lib