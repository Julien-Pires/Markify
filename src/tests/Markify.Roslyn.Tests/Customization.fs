module Customization
    open System
    open System.IO
    open System.Reflection

    open Markify.Models.IDE;

    open Ploeh.AutoFixture

    type ProjectContextCustomization (sourceFiles) =
        let sourceFiles = sourceFiles

        interface ICustomization with
            member this.Customize (fixture : IFixture) = 
                fixture.Register<Project> (fun c -> 
                    let project = {
                        Name = "Test"
                        Path = Uri("c:/Test/Test.csproj")
                        Language = ProjectLanguage.CSharp
                        Files = sourceFiles |> Seq.map (fun c -> Uri(this.CreateFullPath(c))) }
                    project)

        member this.CreateFullPath path =
            let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
            let cleanPath = Uri.UnescapeDataString (basePath.Path)

            Path.Combine (Path.GetDirectoryName (cleanPath), path)