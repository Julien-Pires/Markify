namespace Markify.Roslyn.Tests

open System
open System.IO
open System.Reflection
open System.Xml.Serialization

open Markify.Roslyn
open Markify.Models.IDE;

open Ploeh.AutoFixture

[<XmlRoot("Project")>]
type TestProject() =
    [<XmlArray("Files")>]
    [<XmlArrayItem("Uri")>]
    member val Files : string[] = [||] with get, set

type ProjectContextCustomization (projectFile, language) =
    let projectFile = projectFile
    let language = language

    let getFullPath path =
        let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
        let cleanPath = Uri.UnescapeDataString (basePath.Path)

        Path.Combine (Path.GetDirectoryName (cleanPath), path)

    let readXml path =
        let fullPath = getFullPath (sprintf "Projects/%s" path)
        let serializer = XmlSerializer(typeof<TestProject>)
        use stream = new StreamReader(fullPath)
        serializer.Deserialize stream :?> TestProject

    let getProjectExt language =
        match language with
        | ProjectLanguage.CSharp -> "csproj"
        | ProjectLanguage.VisualBasic -> "vbproj"
        | _ -> ""

    let getFileExt language =
        match language with
        | ProjectLanguage.CSharp -> "cs"
        | ProjectLanguage.VisualBasic -> "vb"
        | _ -> ""

    interface ICustomization with
        member this.Customize (fixture : IFixture) =
            fixture.Register<SourceReader> (fun c ->
                SourceReader(seq {
                    yield LanguageReader(CSharpHelper(), ["cs"])
                    yield LanguageReader(VisualBasicHelper(), ["vb"])
                }))
            fixture.Register<Project> (fun c ->
                let testProject = readXml projectFile
                let project = {
                    Name = "Test"
                    Path = Uri(sprintf "c:/Test/Test.%s" (getProjectExt language))
                    Language = language
                    Files = 
                        testProject.Files
                        |> Seq.map (fun c -> sprintf "Projects/%s" c)
                        |> Seq.map (fun c -> sprintf "%s.%s" (getFullPath c) (getFileExt language))
                        |> Seq.map Uri}
                project)