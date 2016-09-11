namespace Markify.Roslyn.Tests

module Roslyn_Processor_Process_Types_Inheritance_Tests =
    open Markify.Roslyn

    open Markify.Models.IDE
    open Markify.Models.Definitions

    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "SingleClass", 0)>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "InheritClass", 1)>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "ImplementGenInterfaceClass", 2)>]  
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "ISingleInterface", 0)>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IImplementGenericInterface", 2)>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "SingleStruct", 0)>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "ImplementIDisposable", 1)>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "IntEnum", 1)>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "SingleClass", 0)>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "InheritClass", 1)>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "ImplementGenInterfaceClass", 2)>]  
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "ISingleInterface", 0)>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "IImplementGenericInterface", 2)>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "SingleStruct", 0)>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "ImplementIDisposable", 1)>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "IntEnum", 1)>]
    let ``Process project should return base types when type has one`` (name, count, sut: RoslynAnalyzer, project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ count = Seq.length typeDef.BaseTypes @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "InheritClass", "Exception")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "MixedInheritanceClass", "IDisposable;Exception")>]   
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IImplementIDisposable", "IDisposable")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IImplementGenericInterface", "IList<String>;IReadOnlyCollection<String>")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "ImplementIDisposable", "IDisposable")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "ImplementGenericInterface", "IList<String>;IReadOnlyCollection<String>")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "IntEnum", "int")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "ByteEnum", "byte")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "InheritClass", "Exception")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "MixedInheritanceClass", "IDisposable;Exception")>]   
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "IImplementIDisposable", "IDisposable")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "IImplementGenericInterface", "IList(Of String);IReadOnlyCollection(Of String)")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "ImplementIDisposable", "IDisposable")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "ImplementGenericInterface", "IList(Of String);IReadOnlyCollection(Of String)")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "IntEnum", "Integer")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "ByteEnum", "Byte")>]
    let ``Process project should return base types when type has multiple`` (name, typeNames: string, sut: RoslynAnalyzer, project) =
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