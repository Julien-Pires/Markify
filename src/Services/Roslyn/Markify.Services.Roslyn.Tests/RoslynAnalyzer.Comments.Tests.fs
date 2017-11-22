namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerCommentsTests =
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    let getComments = function
        | Class x | Struct x | Interface x -> x.Comments.Comments
        | Enum x -> x.Comments.Comments
        | Delegate x -> x.Comments.Comments

    let getComment commentName definition =
        getComments definition
        |> Seq.tryFind (fun c -> c.Name = commentName)
    
    [<Theory>]
    [<ProjectData("Comments", "EnumWithoutComments", "Foo")>]
    [<ProjectData("Comments", "ClassWithoutComments`2", "Foo")>]
    [<ProjectData("Comments", "StructWithoutComments`2", "Foo")>]
    [<ProjectData("Comments", "InterfaceWithoutComments`2", "Foo")>]
    [<ProjectData("Comments", "DelegateWithoutComments`2", "Foo")>]
    let ``Analyze should return comment when comment does not exist on type`` (name, commentName, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment commentName
        
        test <@ actual.IsNone @>
        
    [<Theory>]
    [<ProjectData("Comments", "EnumWithSimpleComments", "summary")>]
    [<ProjectData("Comments", "ClassWithSimpleComments`2", "summary")>]
    [<ProjectData("Comments", "StructWithSimpleComments`2", "summary")>]
    [<ProjectData("Comments", "InterfaceWithSimpleComments`2", "summary")>]
    [<ProjectData("Comments", "DelegateWithSimpleComments`2", "summary")>]
    let ``Analyze should return comment when paired tag comment exists on type`` (name, commentName, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual = 
            library
            |> LibraryHelper.getDefinition name
            |> getComment commentName
        
        test <@ actual.IsSome @>

    [<Theory>]
    [<ProjectData("Comments", "EnumWithSimpleComments", "inheritdoc")>]
    [<ProjectData("Comments", "ClassWithSimpleComments`2", "inheritdoc")>]
    [<ProjectData("Comments", "StructWithSimpleComments`2", "inheritdoc")>]
    [<ProjectData("Comments", "InterfaceWithSimpleComments`2", "inheritdoc")>]
    [<ProjectData("Comments", "DelegateWithSimpleComments`2", "inheritdoc")>]
    let ``Analyze should return comment when unpaired tag comment exists on type``(name, commentName, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComment commentName

        test <@ actual.IsSome @>

    [<Theory>]
    [<ProjectData("Comments", "ClassWithSimpleComments`2", "typeparam")>]
    [<ProjectData("Comments", "StructWithSimpleComments`2", "typeparam")>]
    [<ProjectData("Comments", "InterfaceWithSimpleComments`2", "typeparam")>]
    [<ProjectData("Comments", "DelegateWithSimpleComments`2", "typeparam")>]
    let ``Analyze should return multiple comments when type has multiple identical comments``(name, commentName, sut : RoslynAnalyzer, project : ProjectInfo) =
        let library = (sut :> IProjectAnalyzer).Analyze project.Project
        let actual =
            library
            |> LibraryHelper.getDefinition name
            |> getComments
            |> Seq.filter (fun c -> c.Name = commentName)

        test <@ actual |> Seq.length > 1 @>