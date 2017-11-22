namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Ide
open System
open System.IO
open System.Reflection

module ProjectHelper =
    let getFileExtension = function
        | ProjectLanguage.CSharp -> "cs"
        | ProjectLanguage.VisualBasic -> "vb"
        | _ -> ""

    let getProjectExtension = function
        | ProjectLanguage.CSharp -> "csproj"
        | ProjectLanguage.VisualBasic -> "vbproj"
        | _ -> ""

    let getFullPath path =
        let basePath = UriBuilder (Assembly.GetExecutingAssembly().CodeBase)
        let cleanPath = Uri.UnescapeDataString (basePath.Path)
        Path.Combine (Path.GetDirectoryName (cleanPath), path)

open System.Xml.Serialization
open Ploeh.AutoFixture
open Markify.Services.Roslyn.Fixtures

module ProjectReader =
    let (|IsValidSolution|_|) (path : string) =
        match path.Split([|'/'|], StringSplitOptions.RemoveEmptyEntries) with
        | [|x; y|] -> Some (x, y)
        | _ -> None

    let readXml (path : string) =
        let serializer = XmlSerializer(typeof<Projects>)
        use stream = new StreamReader(path)
        serializer.Deserialize stream :?> Projects

    let read path =
        match path with
        | IsValidSolution (file, project) -> 
            (sprintf "Projects/%s.xml" file)
            |> ProjectHelper.getFullPath
            |> readXml
            |> fun c -> c.All |> Seq.find (fun d -> d.Name = project)
        | _ -> raise (ArgumentException("Incorrect path", "path"))

type ProjectCustomization (file, language) =
    let extension = ProjectHelper.getFileExtension language
    let projectExtension = ProjectHelper.getProjectExtension language

    let buildProject (content : ProjectContent) = 
        let project = {   
            Name = "Test"
            Path = Uri(sprintf "c:/Test/Test.%s" projectExtension)
            Language = language
            Files =
                content.Files
                |> Seq.map (fun c ->
                    let fullpath = ProjectHelper.getFullPath <| sprintf "Projects/%s" c
                    match language with                
                    | ProjectLanguage.Unsupported -> fullpath
                    | _ -> sprintf "%s.%s" fullpath extension)
                |> Seq.map Uri }
        {   Project = project
            Count = content.Count }
    
    let create = ProjectReader.read >> buildProject

    interface ICustomization with
        member this.Customize (fixture : IFixture) =
            fixture.Inject <| create file