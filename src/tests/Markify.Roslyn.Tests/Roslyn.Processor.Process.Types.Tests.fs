module Roslyn_Processor_Process_Types_Tests
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/EmptySource.cs"|], 0, StructureKind.Class)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], 4, StructureKind.Class)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InterfaceSamples.cs"|], 4, StructureKind.Interface)>]
    let ``Process project with unique structure kind in source`` (count, kind, sut: RoslynProcessor, project: ProjectContext) = 
        let library = (sut :> IProjectProcessor).Process project

        test <@ Seq.length library.Types = count @>
        test <@ library.Types |> Seq.filter (fun c -> c.Kind = kind) |> Seq.length = count @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"; "Projects/Source/Class/ClassSamples.cs"|], "ParentClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InterfaceSamples.cs"; "Projects/Source/Interface/InterfaceSamples.cs"|], "IParentInterface")>]
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
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InterfaceSamples.cs"|], "IInNamespaceInterface")>]
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
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InterfaceSamples.cs"|], "FooSpace.IInNamespaceInterface")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Generics/GenericClass.cs"|], "GenericClass`2")>]
    let ``Process project with types with correct fullname`` (fullname, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity.Fullname = fullname)

        test <@ typeDef.IsSome @>