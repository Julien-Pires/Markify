module Roslyn_Processor_Process_Types_Tests
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/EmptySource.cs"|], 0)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], 4)>]
    let ``Process project with various type`` (count, sut: RoslynProcessor, project: ProjectContext) = 
        let library = (sut :> IProjectProcessor).Process project

        test <@ count = Seq.length library.Types @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"; "Projects/Source/Class/ClassSamples.cs"|], "ParentClass")>]
    let ``Process project without duplicate types`` (fullname, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process project)
            |> (fun c -> c.Types)
            |> Seq.filter (fun c -> c.Identity.Fullname = fullname)

        test <@ Seq.length typeDef = 1 @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "ParentClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "InNamespaceClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "GenericClass`2")>]
    let ``Process project with types with correct name`` (name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity.Name = name)

        test <@ typeDef.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "SingleClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "ParentClass.NestedClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "FooSpace.InNamespaceClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "GenericClass`2")>]
    let ``Process project with types with correct fullname`` (fullname, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity.Fullname = fullname)

        test <@ typeDef.IsSome @>