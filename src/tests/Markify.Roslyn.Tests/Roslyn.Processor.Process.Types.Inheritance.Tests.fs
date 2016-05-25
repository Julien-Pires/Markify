module Roslyn_Processor_Process_Types_Inheritance_Tests
    open Processor
    open Markify.Models.Context
    open Markify.Models.Definitions
    open Markify.Processors

    open Xunit
    open Swensen.Unquote
    open Markify.Fixtures

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/ClassSamples.cs"|], "SingleClass", 0)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/InheritedClass.cs"|], "InheritClass", 1)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/InheritedClass.cs"|], "ImplementGenInterfaceClass", 2)>]  
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InterfaceSamples.cs"|], "ISingleInterface", 0)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InheritedInterface.cs"|], "IImplementGenericInterface", 2)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/StructSamples.cs"|], "SingleStruct", 0)>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/InheritedStruct.cs"|], "ImplementIDisposable", 1)>]
    let ``Process project with type with inheritance tree`` (name, count, sut: RoslynProcessor, project: ProjectContext) =
        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ count = Seq.length typeDef.BaseTypes @>

    [<Theory>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/InheritedClass.cs"|], "InheritClass", "Exception")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Class/InheritedClass.cs"|], "MixedInheritanceClass", "IDisposable Exception")>]   
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InheritedInterface.cs"|], "IImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Interface/InheritedInterface.cs"|], "IImplementGenericInterface", "IList<String> IReadOnlyCollection<String>")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/InheritedStruct.cs"|], "ImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData([|"Projects/Source/Struct/InheritedStruct.cs"|], "ImplementGenericInterface", "IList<String> IReadOnlyCollection<String>")>]
    let ``Process project with type with multiple inheritance`` (name, typeNames: string, sut: RoslynProcessor, project: ProjectContext) =
        let expectedBaseTypes = typeNames.Split [|' '|]

        let typeDef = 
            (sut :> IProjectProcessor)
            |> (fun c -> c.Process(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let baseTypes = typeDef.BaseTypes |> Seq.filter (fun c -> Seq.contains c expectedBaseTypes)

        test <@ Seq.length expectedBaseTypes = Seq.length baseTypes @>