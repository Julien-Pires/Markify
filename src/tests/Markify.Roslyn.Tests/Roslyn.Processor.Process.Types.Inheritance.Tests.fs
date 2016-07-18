module Roslyn_Processor_Process_Types_Inheritance_Tests
    open Markify.Roslyn

    open Markify.Models.IDE
    open Markify.Models.Definitions

    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "SingleClass", 0)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "InheritClass", 1)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ImplementGenInterfaceClass", 2)>]  
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "ISingleInterface", 0)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IImplementGenericInterface", 2)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "SingleStruct", 0)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "ImplementIDisposable", 1)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "IntEnum", 1)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "SingleClass", 0)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "InheritClass", 1)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "ImplementGenInterfaceClass", 2)>]  
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "ISingleInterface", 0)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IImplementGenericInterface", 2)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "SingleStruct", 0)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "ImplementIDisposable", 1)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "IntEnum", 1)>]
    let ``Process project with type with inheritance tree`` (name, count, sut: RoslynAnalyzer, project: Project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ count = Seq.length typeDef.BaseTypes @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "InheritClass", "Exception")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "MixedInheritanceClass", "IDisposable;Exception")>]   
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IImplementGenericInterface", "IList<String>;IReadOnlyCollection<String>")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "ImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "ImplementGenericInterface", "IList<String>;IReadOnlyCollection<String>")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "IntEnum", "int")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "ByteEnum", "byte")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "InheritClass", "Exception")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "MixedInheritanceClass", "IDisposable;Exception")>]   
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IImplementGenericInterface", "IList(Of String);IReadOnlyCollection(Of String)")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "ImplementIDisposable", "IDisposable")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "ImplementGenericInterface", "IList(Of String);IReadOnlyCollection(Of String)")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "IntEnum", "Integer")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "ByteEnum", "Byte")>]
    let ``Process project with type with multiple inheritance`` (name, typeNames: string, sut: RoslynAnalyzer, project: Project) =
        let expectedBaseTypes = typeNames.Split [|';'|]

        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let baseTypes = 
            typeDef.BaseTypes 
            |> Seq.filter (fun c -> Seq.contains c expectedBaseTypes)

        test <@ Seq.length expectedBaseTypes = Seq.length baseTypes @>