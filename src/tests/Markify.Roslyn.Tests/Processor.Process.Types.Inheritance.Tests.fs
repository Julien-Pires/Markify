module Processor_Process_Types_Inheritance_Tests
    open Processor
    open Markify.Models
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/ClassSamples.cs"|], "SingleClass", 0)>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "InheritClass", 1)>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "ImplementInterfaceClass", 1)>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "ImplementGenInterfaceClass", 2)>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "MixedInheritanceClass", 3)>]
    let ``Process project with type with inheritance tree`` (name, count, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.MemberName = name)

        test <@ count = Seq.length typeDef.BaseTypes @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "InheritClass", "Exception")>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "ImplementInterfaceClass", "IDisposable")>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "ImplementGenInterfaceClass", "IList<String>")>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "MixedInheritanceClass", "Exception")>]
    [<ProjectContextInlineAutoData([|"Class/InheritedClass.cs"|], "MixedInheritanceClass", "IDisposable")>]
    let ``Process project with type with multiple inheritance`` (name, expectedType, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.MemberName = name)
        let foundType =
            typeDef.BaseTypes
            |> Seq.tryFind (fun c -> c = expectedType)

        test <@ foundType.IsSome @>