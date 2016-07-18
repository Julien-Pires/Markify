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
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, 0, "SingleClass")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 2, "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 2, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 1, "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 1, "Do`1")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, 0, "SingleClass")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 2, "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 2, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 1, "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 1, "Do`1")>]
    let ``Process project with generic types`` (count, name, sut: RoslynAnalyzer, project: Project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.length typeDef.Parameters = count @>

    [<Theory>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "Do`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "Do`1")>]
    let ``Process project with generic parameter with correct parameter name`` (parameterName, name, sut: RoslynAnalyzer, project: Project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.tryFind (fun c -> c.Identity.Name = parameterName)

        test <@ parameter.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "", "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "in", "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "out", "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "", "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "in", "T", "Do`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "", "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "In", "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Out", "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "", "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "In", "T", "Do`1")>]
    let ``Process project with generic parameters with modifiers`` (modifier: string, parameterName, name, sut: RoslynAnalyzer, project: Project) = 
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)

        test <@ modifier = parameter.Modifier @>

    [<Theory>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 2, "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 0, "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 1, "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, 3, "T", "Do`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 2, "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 0, "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 1, "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, 3, "T", "Do`1")>]
    let ``Process project with generic parameter with constraints`` (count, parameterName, name, sut: RoslynAnalyzer, project: Project) =
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)

        test <@ Seq.length parameter.Constraints = count @>

    [<Theory>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "class;IList<string>", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "Y", "", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "struct", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "T", "class;IDisposable;new()", "Do`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "Class;IList(Of String)", "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Y", "", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "Structure", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "T", "Class;IDisposable;New", "Do`1")>]
    let ``Process project with generic parameter with correct constraints names`` (parameterName, constraints: string, name, sut: RoslynAnalyzer, project: Project) =
        let expectedConstraints = constraints.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)
        let parameterConstraints = 
            parameter.Constraints 
            |> Seq.filter (fun c -> Seq.contains c expectedConstraints)

        test <@ Seq.length expectedConstraints = Seq.length parameterConstraints @>