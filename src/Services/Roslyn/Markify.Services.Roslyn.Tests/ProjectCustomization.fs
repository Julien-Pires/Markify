namespace Markify.Services.Roslyn.Tests

open System
open System.IO
open System.Reflection
open System.Xml.Serialization
open Markify.Domain.Ide
open Markify.Services.Roslyn.Fixtures

module ProjectBuilder =
    let private projectsIndex = "Projects/Projects.xml"

    let private getFileExtension = function
        | ProjectLanguage.CSharp -> "cs"
        | ProjectLanguage.VisualBasic -> "vb"
        | _ -> ""

    let private getProjectExtension = function
        | ProjectLanguage.CSharp -> "csproj"
        | ProjectLanguage.VisualBasic -> "vbproj"
        | _ -> ""

    let private getFullPath path =
        let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
        let cleanPath = Uri.UnescapeDataString (basePath.Path)
        Path.Combine (Path.GetDirectoryName (cleanPath), path)

    let private readXml (path : string) =
        let serializer = XmlSerializer(typeof<Projects>)
        use fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
        use reader = new StreamReader(fileStream)
        serializer.Deserialize reader :?> Projects

    let private findProject projectName =
        match String.IsNullOrWhiteSpace(projectName) with
        | false ->
            projectsIndex
            |> getFullPath
            |> readXml
            |> fun c -> c.All |> Seq.tryFind (fun d -> d.Name = projectName)
        | true -> raise (ArgumentException("Incorrect project name", "projectName"))

    let create projectName language = 
        match findProject projectName with
        | Some x -> 
            { Name = projectName
              Path = Uri(sprintf "c:/%s/%s.%s" projectName projectName (getProjectExtension language))
              Language = language
              Files =
                x.Files
                |> Seq.map (fun c ->
                    let fullpath = getFullPath <| sprintf "Projects/%s" c
                    match language with                
                    | ProjectLanguage.Unsupported -> fullpath
                    | _ -> sprintf "%s.%s" fullpath (getFileExtension language))
                |> Seq.map Uri }
        | None -> raise (ArgumentException(sprintf "%s is not a valid project" projectName, projectName))