module Processor_Process_Types_Generics_Tests
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
            |> Seq.find (fun c -> c.MemberName = name)

        test <@ count = Seq.length typeDef.GenericParameters @>