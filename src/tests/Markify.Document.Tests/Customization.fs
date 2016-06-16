module Customization

    open System
    open System.Collections.Generic

    open Markify.Document
    open Markify.Core.Processors
    open Markify.Models.Document
    open Markify.Models.Definitions

    open Ploeh.AutoFixture

    type SimpleDocumentCustomization (root, projectName, projectCount, typeCount, extension) =
        let root = root
        let projectName = projectName;
        let projectCount = projectCount
        let typeCount = typeCount

        interface ICustomization with
            member  this.Customize (fixture : IFixture) = 
                fixture.Inject<IDocumentOrganizer> (new SimpleDocumentProcessor())

                fixture.Register<DocumentSetting> (fun c -> 
                    let setting  = {
                        Root = new Uri(root)
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
                            Fullname = sprintf "Parent.Type%i" c }
                        let typeDef = {
                            Identity = identity
                            Kind = StructureKind.Class
                            AccessModifiers = Seq.empty
                            Modifiers = Seq.empty
                            BaseTypes = Seq.empty
                            Parameters = Seq.empty }
                        typeDef)

            { Project = name; Types = types }