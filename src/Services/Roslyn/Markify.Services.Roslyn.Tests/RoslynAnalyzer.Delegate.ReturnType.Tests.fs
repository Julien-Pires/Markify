namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerDelegateReturnTypeTests =
    open Markify.Services.Roslyn
    open Markify.Domain.Compiler
    open Markify.Domain.Ide
    open Swensen.Unquote
    open Xunit

    [<Theory>]
    [<ProjectData("Delegates", "WithoutParameters", "void")>]
    [<ProjectData("Delegates", "WithReturnType", "Int32")>]
    [<ProjectData("Delegates", "WithGenericReturnType`1", "T")>]
    let ``Analyze should return expected delegate return type`` (name, returnType, sut : RoslynAnalyzer, project) =
        let expected = LanguageHelperOld.getType project.Language returnType
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = 
            TestHelper.getDefinition name library 
            |> DefinitionsHelper.getReturnType

        test <@ actual = Some expected @>