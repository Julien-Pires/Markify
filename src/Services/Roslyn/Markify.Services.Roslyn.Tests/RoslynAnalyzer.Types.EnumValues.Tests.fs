namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesEnumValuesTests =
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote
    
    [<Theory>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "BarType", 0)>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "FooType", 4)>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "BarType", 0)>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "FooType", 4)>]
    let ``Analyze should return correct enum value count`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getEnumValues
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "FooType", "Foo")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "FooType", "FooBar")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "FooType", "Foo")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "FooType", "FooBar")>]
    let ``Analyze should return correct enum value name`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let value =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getEnumValues
                    |> Seq.tryFind (fun d -> d.Name = expected)
                value::acc) []

        test <@ actual |> List.forall ((<>) None) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "FooType" , "Foo", "0")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.CSharp, "FooType" , "BarFoo", "")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "FooType" , "Foo", "0")>]
    [<MultiProjectData("TypeMembers/EnumValues", ProjectLanguage.VisualBasic, "FooType" , "BarFoo", "")>]
    let ``Analyze should return correct enum underlying value`` (name, valueName, value, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = 
            match value with
            | "" -> None
            | x -> Some x
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let enumValue =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getEnumValues
                    |> Seq.find (fun d -> d.Name = valueName)
                enumValue.Value::acc) []

        test <@ actual |> List.forall ((=) expected) @>