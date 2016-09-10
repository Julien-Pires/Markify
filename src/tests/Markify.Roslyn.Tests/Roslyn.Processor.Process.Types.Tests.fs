module Roslyn_Processor_Process_Types_Tests
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Models.Definitions
    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    let getName name = 
        match name with
        | "" -> None
        | _ -> Some name

    [<Theory>]
    [<ProjectData("EmptySourceProject", ProjectLanguage.CSharp, 0, StructureKind.Class)>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, 24, StructureKind.Class)>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, 15, StructureKind.Interface)>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, 13, StructureKind.Struct)>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, 10, StructureKind.Enum)>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, 8, StructureKind.Delegate)>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, 24, StructureKind.Class)>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, 15, StructureKind.Interface)>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, 13, StructureKind.Struct)>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, 10, StructureKind.Enum)>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, 8, StructureKind.Delegate)>]
    let ``Process projects should return expected types count`` (expected, kind, sut : RoslynAnalyzer, project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let typesCount =
            library.Types
            |> Seq.filter (fun c -> c.Kind = kind)
            |> Seq.length

        test <@ typesCount = expected @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "ParentClass", "", "")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IParentInterface", "", "")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "NestedStruct", "ParentStruct", "")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "SingleDelegate", "", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "INestedInterface", "IParentInterface", "")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "SingleStruct", "", "")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "SingleEnum", "", "")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "NestedDelegate", "ParentClass", "")>]
    let ``Process project should return no duplicate types`` (name, parents, nspace, sut: RoslynAnalyzer, project) =
        let fullname = {
            Name = name
            Parents = getName parents
            Namespace = getName nspace
        }
        let typeDef =
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.filter (fun c -> c.Identity = fullname)

        test <@ Seq.length typeDef = 1 @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "ParentClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "InNamespaceClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IInNamespaceInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "NestedStruct")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "NestedEnum")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "InNamespaceDelegate")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "Do`1")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "ParentClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "InNamespaceClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "IInNamespaceInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "NestedStruct")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "NestedEnum")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "InNamespaceDelegate")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "GenericClass`2")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "Do`1")>]
    let ``Process project should return type with correct name`` (name, sut: RoslynAnalyzer, project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity.Name = name)

        test <@ typeDef.IsSome @>
    
    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "SingleClass", "", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "NestedClass", "ParentClass", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "ChildClass", "AnotherParentClass", "FooSpace.InnerSpace")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "IInNamespaceInterface", "", "FooSpace")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "InNamespaceStruct", "", "FooSpace")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "InNamespaceDelegate", "", "FooSpace")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "GenericClass`2", "", "")>]
    [<ProjectData("GenericsProject", ProjectLanguage.CSharp, "Do`1", "", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "SingleClass", "", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "NestedClass", "ParentClass", "")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "ChildClass", "AnotherParentClass", "FooSpace.InnerSpace")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "IInNamespaceInterface", "", "FooSpace")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "InNamespaceStruct", "", "FooSpace")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "InNamespaceDelegate", "", "FooSpace")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "GenericClass`2", "", "")>]
    [<ProjectData("GenericsProject", ProjectLanguage.VisualBasic, "Do`1", "", "")>]
    let ``Process project should return types with correct fullname`` (name, parents, nspace, sut: RoslynAnalyzer, project) =
        let fullname = {
            Name = name
            Parents = getName parents
            Namespace = getName nspace
        }
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity = fullname)

        test <@ typeDef.IsSome @>

    [<Theory>]
    [<ProjectData("InvalidFileProject", ProjectLanguage.CSharp, 0)>]
    [<ProjectData("InvalidFileProject", ProjectLanguage.VisualBasic, 0)>]
    let ``Process project should return nothing when file does not exists`` (expected, sut : RoslynAnalyzer, project) =
        let library = (sut :> IProjectAnalyzer).Analyze project
        let actual = Seq.length library.Types

        test <@ actual = expected @>