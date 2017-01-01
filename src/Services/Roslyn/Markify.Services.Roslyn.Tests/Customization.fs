namespace Markify.Services.Roslyn.Tests

open System
open System.IO
open System.Reflection
open System.Xml.Serialization
open Markify.Roslyn
open Markify.Domain.Ide;
open Ploeh.AutoFixture

[<XmlRoot("Project")>]
type ProjectContent() =
    [<XmlArray("Files")>]
    [<XmlArrayItem("Uri")>]
    member val Files : string[] = [||] with get, set

    [<XmlElement("Count")>]
    member val Count : int = 0 with get, set

[<XmlRoot("Solution")>]
type SolutionContent() =
    [<XmlArray("Projects")>]
    [<XmlArrayItem("Project")>]
    member val Projects : ProjectContent[] = [||] with get, set

type ProjectInfo = {
    Project : Project
    Count : int
}

type ProjectContextCustomization (file, language) =
    let file = file

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

    let createProject (content : ProjectContent) = 
        let project = {   
            Name = "Test"
            Path = Uri(sprintf "c:/Test/Test.%s" projectExtension)
            Language = language
            Files =
                content.Files
                |> Seq.map (fun c -> 
                    let fullpath = getFullPath <| sprintf "Projects/%s" c
                    match language with                
                    | ProjectLanguage.Unsupported -> fullpath
                    | _ -> sprintf "%s.%s" fullpath extension)
                |> Seq.map Uri }
        {   Project = project
            Count = content.Count }

    member this.ReadXml<'T> path =
        let fullPath = getFullPath (sprintf "Projects/%s" path)
        let serializer = XmlSerializer(typeof<'T>)
        use stream = new StreamReader(fullPath)
        serializer.Deserialize stream :?> 'T
        
    member this.GetProjects path =
        (this.ReadXml<SolutionContent> (sprintf "%s.xml" path)).Projects

    member this.BuildMultiProjects path =
        this.GetProjects path
        |> Array.map (createProject)

    interface ICustomization with
        member this.Customize (fixture : IFixture) =
            fixture.Inject (this.BuildMultiProjects file)