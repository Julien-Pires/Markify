﻿module Processor_Process_Types_Tests
    open Processor
    open Markify.Models
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"EmptySource.cs"|], 0)>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], 4)>]
    let ``Process project with various type`` (count, sut: RoslynProcessor, project: ProjectContext) = 
        let library = (sut :> IProjectProcessor).Process(project)

        test <@ count = Seq.length library.Types @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "ParentClass")>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "InNamespaceClass")>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], "GenericClass`2")>]
    let ``Process project with types with correct name`` (name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.MemberName = name)

        test <@ typeDef.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "SingleClass", "SingleClass")>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "NestedClass","ParentClass.NestedClass")>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "InNamespaceClass","FooSpace.InNamespaceClass")>]
    [<ProjectContextInlineAutoData([|"Generics/GenericClass.cs"|], "GenericClass`2","GenericClass`2")>]
    let ``Process project with types with correct fullname`` (name, fullname, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.MemberName = name)

        test <@ typeDef.IsSome @>
        test <@ fullname = typeDef.Value.Fullname @>