module Roslyn_Processor_Process_Types_Modifiers_Tests
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AccessModifier.cs"|], "public", "PublicClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AccessModifier.cs"|], "internal", "InternalClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AccessModifier.cs"|], "protected", "ProtectedClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AccessModifier.cs"|], "protected internal", "ProtectedInternalClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AccessModifier.cs"|], "private", "PrivateClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/AccessModifier.cs"|], "public", "IPublicInterface")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/AccessModifier.cs"|], "internal", "IInternalInterface")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/AccessModifier.cs"|], "protected internal", "IProtectedInternalInterface")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/AccessModifier.cs"|], "public", "PublicStruct")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/AccessModifier.cs"|], "internal", "InternalStruct")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/AccessModifier.cs"|], "protected internal", "ProtectedInternalStruct")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Enum/AccessModifier.cs"|], "public", "PublicEnum")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Enum/AccessModifier.cs"|], "internal", "InternalEnum")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Enum/AccessModifier.cs"|], "protected internal", "ProtectedInternalEnum")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Delegate/AccessModifier.cs"|], "public", "PublicDelegate")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Delegate/AccessModifier.cs"|], "internal", "InternalDelegate")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Delegate/AccessModifier.cs"|], "protected internal", "ProtectedInternalDelegate")>]
    let ``Process project with types that have access modifiers`` (modifier: string, name, sut: RoslynProcessor, project: ProjectContext) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = 
            typeDef.AccessModifiers 
            |> Seq.filter (fun c -> Seq.contains c expectedModifiers) 

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "abstract", "AbstractClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "sealed", "SealedClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "partial", "PartialClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "static", "StaticClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/AdditionnalModifier.cs"|], "partial", "IPartialInterface")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/AdditionnalModifier.cs"|], "partial", "PartialStruct")>]
    let ``Process project with types that have a single modifier`` (modifier, name, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.contains modifier typeDef.Modifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "abstract partial", "AbstractPartialClass")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/AdditionnalModifier.cs"|], "sealed partial", "SealedPartialClass")>]
    let ``Process project with types that have multiple modifiers`` (modifier: string, name, sut: RoslynProcessor, project: ProjectContext) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = Seq.filter (fun c -> Seq.contains c expectedModifiers) typeDef.Modifiers

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>