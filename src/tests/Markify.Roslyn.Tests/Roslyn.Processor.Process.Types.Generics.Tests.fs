module Roslyn_Processor_Process_Types_Generics_Tests
    open System
    
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Core.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], 0, "SingleClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], 2, "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], 2, "GenericClass`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericStruct.cs"|], 1, "GenericStruct`1")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericDelegate.cs"|], 1, "Do`1")>]
    let ``Process project with generic types`` (count, name, sut: RoslynProcessor, project: Project) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.length typeDef.Parameters = count @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericStruct.cs"|], "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericDelegate.cs"|], "T", "Do`1")>]
    let ``Process project with generic parameter with correct parameter name`` (parameterName, name, sut: RoslynProcessor, project: Project) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.tryFind (fun c -> c.Identity.Name = parameterName)

        test <@ parameter.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "", "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], "in", "T", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], "out", "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericStruct.cs"|], "", "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericDelegate.cs"|], "in", "T", "Do`1")>]
    let ``Process project with generic parameters with modifiers`` (modifier: string, parameterName, name, sut: RoslynProcessor, project: Project) = 
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)

        test <@ modifier = parameter.Modifier @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], 2, "T", "GenericClass`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], 0, "Y", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericStruct.cs"|], 1, "T", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericDelegate.cs"|], 3, "T", "Do`1")>]
    let ``Process project with generic parameter with constraints`` (count, parameterName, name, sut: RoslynProcessor, project: Project) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)

        test <@ Seq.length parameter.Constraints = count @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "T", "class IList<string>", "GenericClass`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericInterface.cs"|], "Y", "", "GenericInterface`2")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericStruct.cs"|], "T", "struct", "GenericStruct`1")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericDelegate.cs"|], "T", "class IDisposable new()", "Do`1")>]
    let ``Process project with generic parameter with correct constraints names`` (parameterName, constraints: string, name, sut: RoslynProcessor, project: Project) =
        let expectedConstraints = constraints.Split ([|' '|], StringSplitOptions.RemoveEmptyEntries)
        
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let parameter =
            typeDef.Parameters
            |> Seq.find (fun c -> c.Identity.Name = parameterName)
        let parameterConstraints = 
            parameter.Constraints 
            |> Seq.filter (fun c -> Seq.contains c expectedConstraints)

        test <@ Seq.length expectedConstraints = Seq.length parameterConstraints @>