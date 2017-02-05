namespace Markify.Services.Document.Tests

module Customization =
    open System
    open System.Collections.Generic
    open Markify.Document
    open Markify.Domain.Compiler
    open Markify.Domain.Document
    open Markify.Services.Document
    open Ploeh.AutoFixture

    type SimpleDocumentCustomization (projectName, projectCount, typeCount, extension) =
        let projectName = projectName;
        let projectCount = projectCount
        let typeCount = typeCount

        interface ICustomization with
            member  this.Customize (fixture : IFixture) = 
                fixture.Inject<IDocumentOrganizer> (SimpleDocumentOrganizer())
                fixture.Register<DocumentSetting> (fun c -> 
                    let setting = {
                        PageExtension = extension }
                    setting)
                fixture.Register<IEnumerable<AssemblyDefinition>> (fun c ->
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
                            Name = sprintf "Type%i" c
                            Parents = Some "Parent"
                            Namespace = None
                            AccessModifiers = Seq.empty
                            Modifiers = Seq.empty
                            BaseTypes = Seq.empty
                            Parameters = Seq.empty }
                        Class { 
                            Identity = identity 
                            Fields = []
                            Properties = []
                            Events = [] 
                            Methods = [] })
            let lib = {
                Project = name
                Namespaces = Seq.empty
                Types = types }
            lib