module Roslyn_Processor_Process_Types_Modifiers_Tests
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/AccessModifier.cs"|], "public", "PublicClass")>]
    [<ProjectContextInlineAutoData([|"Class/AccessModifier.cs"|], "internal", "InternalClass")>]
    [<ProjectContextInlineAutoData([|"Class/AccessModifier.cs"|], "protected", "ProtectedClass")>]
    [<ProjectContextInlineAutoData([|"Class/AccessModifier.cs"|], "protected internal", "ProtectedInternalClass")>]
    [<ProjectContextInlineAutoData([|"Class/AccessModifier.cs"|], "private", "PrivateClass")>]
    let ``Process project with types that have access modifiers`` (modifier: string, name, sut: RoslynProcessor, project: ProjectContext) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = Seq.filter (fun c -> Seq.contains c expectedModifiers) typeDef.AccessModifiers

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "abstract", "AbstractClass")>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "sealed", "SealedClass")>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "partial", "PartialClass")>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "static", "StaticClass")>]
    let ``Process project with types that have a single modifier`` (modifier, name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.contains modifier typeDef.Modifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "abstract partial", "AbstractPartialClass")>]
    [<ProjectContextInlineAutoData([|"Class/AdditionnalModifier.cs"|], "sealed partial", "SealedPartialClass")>]
    let ``Process project with types that have multiple modifiers`` (modifier: string, name, sut: RoslynProcessor, project: ProjectContext) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = Seq.filter (fun c -> Seq.contains c expectedModifiers) typeDef.Modifiers

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>