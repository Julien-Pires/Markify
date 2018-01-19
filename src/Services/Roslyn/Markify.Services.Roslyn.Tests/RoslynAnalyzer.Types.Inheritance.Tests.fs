namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesInheritanceTests =
    open System
    open Markify.Services.Roslyn
    open Markify.Domain.Compiler
    open Xunit
    open Swensen.Unquote
    open Markify.Domain.Ide

    [<Theory>]
    [<ProjectData("Inheritance", "InheritType", "Exception")>]
    [<ProjectData("Inheritance", "InheritPrimitiveType", "Int32")>]
    [<ProjectData("Inheritance", "ImplementInterfaceType", "IDisposable")>]
    [<ProjectData("Inheritance", "MixedInheritanceType", "Exception;IDisposable")>]
    [<SingleLanguageProjectData("Inheritance", ProjectLanguage.CSharp, "ImplementGenericInterfaceType", "IList<String>")>]
    [<SingleLanguageProjectData("Inheritance", ProjectLanguage.VisualBasic, "ImplementGenericInterfaceType", "IList(Of String)")>]
    let ``Analyze should return base types when type has some`` (name, types : string, sut : RoslynAnalyzer, project) =
        let expected = Set <| types.Split(';')
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual =
            TestHelper.getDefinitions name library 
            |> Seq.map (fun c -> Set c.Identity.BaseTypes)

        test <@ actual |> Seq.forall ((Set.isSubset) expected) @>