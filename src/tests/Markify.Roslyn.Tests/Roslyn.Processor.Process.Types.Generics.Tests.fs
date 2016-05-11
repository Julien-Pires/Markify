module Roslyn_Processor_Process_Types_Generics_Tests
    open Processor
    open Markify.Models
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], 0, "SingleClass")>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], 2, "GenericClass`2")>]
    let ``Process project with generic types`` (count, name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Name = name)

        test <@ count = Seq.length typeDef.GenericParameters @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], "T", "GenericClass`2")>]
    let ``Process project with generic parameter with correct name`` (parameterName, name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Name = name)
        let parameter =
            typeDef.GenericParameters
            |> Seq.tryFind (fun c -> c.Name = parameterName)

        test <@ parameter.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], "", "T", "GenericClass")>]
    let ``Process project with generic parameters that have modifiers`` (modifier: string, parameterName, name, sut: RoslynProcessor, project: ProjectContext) = 
        let expectedModifier = 
            match modifier with
            | "" -> None
            | _ -> Some(modifier)

        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Name = name)
        let parameter =
            typeDef.GenericParameters
            |> Seq.find (fun c -> c.Name = parameterName)

        test <@ expectedModifier = Some(parameter.Name) @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], 2, "T", "GenericClass")>]
    let ``Process project with generic parameter with constraints`` (count, parameterName, name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Name = name)
        let parameter =
            typeDef.GenericParameters
            |> Seq.find (fun c -> c.Name = parameterName)

        test <@ count = Seq.length parameter.Constraints @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], "T", "class IList<string>", "GenericClass")>]
    let ``Process project with generic parameter with correct constraints names`` (parameterName, constraints: string, name, sut: RoslynProcessor, project: ProjectContext) =
        let expectedConstraints = constraints.Split [|' '|]
        
        let typeDef =
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Name = name)
        let parameter =
            typeDef.GenericParameters
            |> Seq.find (fun c -> c.Name = parameterName)
        let parameterConstraints = parameter.Constraints |> Seq.filter (fun c -> Seq.contains c expectedConstraints)

        test <@ Seq.length expectedConstraints = Seq.length parameterConstraints @>