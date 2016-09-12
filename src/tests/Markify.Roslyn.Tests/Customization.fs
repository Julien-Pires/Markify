namespace Markify.Roslyn.Tests

open System
open System.IO
open System.Reflection
open System.Xml.Serialization

open Markify.Roslyn
open Markify.Models.IDE;

open Ploeh.AutoFixture

[<XmlRoot("Project")>]
type ProjectContent() =
    [<XmlArray("Files")>]
    [<XmlArrayItem("Uri")>]
    member val Files : string[] = [||] with get, set

    [<XmlElement("Count")>]
    member val Count : int = 0 with get, set

type ProjectInfo = {
    Project : Project
    Count : int
}

type ProjectContextCustomization (projectFile, language) =
    let projectFile = sprintf "%s.xml" projectFile
    let extension = 
        match language with
        | ProjectLanguage.CSharp -> "cs"
        | ProjectLanguage.VisualBasic -> "vb"
        | _ -> ""
    let projectExtension =
        match language with
        | ProjectLanguage.CSharp -> "csproj"
        | ProjectLanguage.VisualBasic -> "vbproj"
        | _ -> ""  

    let getFullPath path =
        let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
        let cleanPath = Uri.UnescapeDataString (basePath.Path)
        Path.Combine (Path.GetDirectoryName (cleanPath), path)

    let readXml path =
        let fullPath = getFullPath (sprintf "Projects/%s" path)
        let serializer = XmlSerializer(typeof<ProjectContent>)
        use stream = new StreamReader(fullPath)
        serializer.Deserialize stream :?> ProjectContent

    interface ICustomization with
        member this.Customize (fixture : IFixture) =
            let projectContent = readXml projectFile
            let project = {
                Name = "Test"
                Path = Uri(sprintf "c:/Test/Test.%s" projectExtension)
                Language = language
                Files = 
                    projectContent.Files
                    |> Seq.map (fun c -> 
                        let folder = sprintf "Projects/%s" c
                        sprintf "%s.%s" (getFullPath folder) extension)
                    |> Seq.map Uri}
            let projectInfo = {
                Project = project;
                Count = projectContent.Count }
            fixture.Inject(project)
            fixture.Inject(projectInfo)
            fixture.Inject(
                SourceConverter(
                    [(CSharpHelper() :> NodeHelper, ["cs"]);
                    (VisualBasicHelper() :> NodeHelper, ["vb"])]))