module Roslyn_Processor_Process_Types_Generics_Tests
    open System
    
    open Markify.Roslyn

    open Markify.Models.IDE
    open Markify.Models.Definitions

    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, 0, "SingleClass")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 2, "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 2, "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 1, "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 1, "Do`1")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, 0, "SingleClass")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 2, "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 2, "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 1, "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 1, "Do`1")>]
    let ``Process project should return type with correct generic parameters`` (count, name, sut: RoslynAnalyzer, project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.length typeDef.Parameters = count @>

    [<Theory>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "Do`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "Do`1")>]
    let ``Process project should return correct parameter name when type is generic`` (parameterName, name, sut: RoslynAnalyzer, project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.tryFind (fun c -> c.Identity = parameterName)

        test <@ parameter.IsSome @>
    
    [<Theory>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "", "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "in", "T", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "out", "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "", "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "in", "T", "Do`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "", "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "In", "T", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "Out", "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "", "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "In", "T", "Do`1")>]
    let ``Process project should return modifiers when type is generic`` (modifier: string, parameterName, name, sut: RoslynAnalyzer, project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity = parameterName)

        test <@ modifier = parameter.Modifier @>

    [<Theory>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 2, "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 0, "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 1, "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, 3, "T", "Do`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 2, "T", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 0, "Y", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 1, "T", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, 3, "T", "Do`1")>]
    let ``Process project should return parameters constraints when type is generic`` (count, parameterName, name, sut: RoslynAnalyzer, project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity = parameterName)

        test <@ Seq.length parameter.Constraints = count @>

    [<Theory>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "class;IList<string>", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "Y", "", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "struct", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "T", "class;IDisposable;new()", "Do`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "Class;IList(Of String)", "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "Y", "", "GenericInterface`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "Structure", "GenericStruct`1")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "T", "Class;IDisposable;New", "Do`1")>]
    let ``Process project should return correct constraint name when generic parameters has constraints`` (parameterName, constraints: string, name, sut: RoslynAnalyzer, project) =
        let expectedConstraints = constraints.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity = parameterName)
        let parameterConstraints = 
            parameter.Constraints 
            |> Seq.filter (fun c -> Seq.contains c expectedConstraints)

        test <@ Seq.length expectedConstraints = Seq.length parameterConstraints @>