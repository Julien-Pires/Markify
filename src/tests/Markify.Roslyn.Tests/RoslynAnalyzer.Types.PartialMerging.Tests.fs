namespace Markify.Roslyn.Tests

module RoslynAnalyzerTypesPartialMergingTests =
    open Markify.Roslyn
    open Markify.Models.Definitions
    open Markify.Core.Analyzers
    open Markify.Models.IDE
    open Swensen.Unquote
    open Xunit
    open Attributes

    let getMethods = function
        | Class x | Struct x | Interface x -> x.Methods
        | _ -> Seq.empty

    let getProperties = function
        | Class x | Struct x | Interface x -> x.Properties
        | _ -> Seq.empty

    let getFields = function
        | Class x | Struct x -> x.Fields
        | _ -> Seq.empty

    let getEvents = function
        | Class x | Struct x | Interface x -> x.Events
        | _ -> Seq.empty

    let getModifiers = function
        | Class x | Struct x | Interface x -> x.Identity.Modifiers
        | _ -> Seq.empty

    let getBaseTypes = function
        | Class x | Struct x | Interface x -> x.Identity.BaseTypes
        | _ -> Seq.empty

    [<Theory>]
    [<MultiProjectData("ContainerPartial", ProjectLanguage.CSharp, "FooType", 4)>]
    [<MultiProjectData("InterfacePartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("ContainerPartial", ProjectLanguage.VisualBasic, "FooType", 4)>]
    [<MultiProjectData("InterfacePartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather methods from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getMethods
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>
        
    [<Theory>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather properties from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getProperties
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("ContainerPartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("ContainerPartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather fields from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getFields
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather events from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getEvents
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("ClassPartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("ClassPartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather modifiers from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getModifiers
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.CSharp, "FooType", 2)>]
    [<MultiProjectData("AllTypesPartial", ProjectLanguage.VisualBasic, "FooType", 2)>]
    let ``Analyze should gather inherited types from partial type`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find(fun c -> c.Identity.Name = name)
                    |> getBaseTypes
                methods::acc) []

        test <@ actual |> List.forall(fun c -> (c |> Seq.length) = expected) @>