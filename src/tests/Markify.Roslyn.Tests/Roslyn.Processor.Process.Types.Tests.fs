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
    [<ProjectContextInlineAutoData("EmptySourceProject.xml", ProjectLanguage.CSharp, 0, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, 24, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, 15, StructureKind.Interface)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, 13, StructureKind.Struct)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, 10, StructureKind.Enum)>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, 8, StructureKind.Delegate)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, 24, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, 15, StructureKind.Interface)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, 13, StructureKind.Struct)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, 10, StructureKind.Enum)>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, 8, StructureKind.Delegate)>]
    let ``Process projects with types`` (expected, kind, sut : RoslynAnalyzer, project : Project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let typesCount =
            library.Types
            |> Seq.filter (fun c -> c.Kind = kind)
            |> Seq.length

        test <@ typesCount = expected @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ParentClass", "", "")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IParentInterface", "", "")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "NestedStruct", "ParentStruct", "")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "SingleDelegate", "", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "INestedInterface", "IParentInterface", "")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "SingleStruct", "", "")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "SingleEnum", "", "")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, "NestedDelegate", "ParentClass", "")>]
    let ``Process project without duplicate types`` (name, parents, nspace, sut: RoslynAnalyzer, project: Project) =
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
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ParentClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "InNamespaceClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IInNamespaceInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "NestedStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "NestedEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "InNamespaceDelegate")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "Do`1")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "ParentClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "InNamespaceClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IInNamespaceInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "NestedStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "NestedEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, "InNamespaceDelegate")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Do`1")>]
    let ``Process project with types with correct name`` (name, sut: RoslynAnalyzer, project: Project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c -> c.Identity.Name = name)

        test <@ typeDef.IsSome @>
    
    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "SingleClass", "", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "NestedClass", "ParentClass", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ChildClass", "AnotherParentClass", "FooSpace.InnerSpace")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IInNamespaceInterface", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "InNamespaceStruct", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "InNamespaceDelegate", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "GenericClass`2", "", "")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "Do`1", "", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "SingleClass", "", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "NestedClass", "ParentClass", "")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "InNamespaceClass", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "ChildClass", "AnotherParentClass", "FooSpace.InnerSpace")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IInNamespaceInterface", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "InNamespaceStruct", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "InNamespaceEnum", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, "InNamespaceDelegate", "", "FooSpace")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "GenericClass`2", "", "")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Do`1", "", "")>]
    let ``Process project with types with correct fullname`` (name, parents, nspace, sut: RoslynAnalyzer, project: Project) =
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