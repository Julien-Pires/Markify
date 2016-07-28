module Roslyn_Processor_Process_Types_Tests
    open Markify.Roslyn
    open Markify.Models.IDE
    open Markify.Models.Definitions
    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    let getTypeFullname name parents =
        let parentsName =
            parents
            |> Seq.fold (fun acc c ->
                match acc with
                | "" -> c
                | _ -> sprintf "%s.%s" acc c) ""
        sprintf "%s.%s" name parentsName

    [<Theory>]
    [<ProjectContextInlineAutoData("EmptySourceProject.xml", ProjectLanguage.CSharp, 0, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, 22, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, 15, StructureKind.Interface)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, 13, StructureKind.Struct)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, 10, StructureKind.Enum)>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, 8, StructureKind.Delegate)>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, 22, StructureKind.Class)>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, 15, StructureKind.Interface)>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, 13, StructureKind.Struct)>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, 10, StructureKind.Enum)>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, 8, StructureKind.Delegate)>]
    let ``Process project with single source`` (expected, kind, sut: RoslynAnalyzer, project: Project) = 
        let library = (sut :> IProjectAnalyzer).Analyze project
        let typesCount =
            library.Types
            |> Seq.filter (fun c -> c.Kind = kind)
            |> Seq.length

        test <@ typesCount = expected @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ParentClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "IParentInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "SingleStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "SingleEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "SingleDelegate")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "ParentClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "IParentInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "SingleStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "SingleEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, "SingleDelegate")>]
    let ``Process project without duplicate types`` (name, sut: RoslynAnalyzer, project: Project) =
        let fullname = getTypeFullname name Seq.empty
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.filter (fun c -> 
                let typeFullname = getTypeFullname c.Identity.Name c.Identity.Parents
                typeFullname = fullname)

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
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "SingleClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "ParentClass.NestedClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "FooSpace.InNamespaceClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "FooSpace.IInNamespaceInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "FooSpace.InNamespaceStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "FooSpace.InNamespaceEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "FooSpace.InNamespaceDelegate")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.CSharp, "Do`1")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "SingleClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "ParentClass.NestedClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.VisualBasic, "FooSpace.InNamespaceClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.VisualBasic, "FooSpace.IInNamespaceInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.VisualBasic, "FooSpace.InNamespaceStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.VisualBasic, "FooSpace.InNamespaceEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.VisualBasic, "FooSpace.InNamespaceDelegate")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "GenericClass`2")>]
    [<ProjectContextInlineAutoData("GenericsProject.xml", ProjectLanguage.VisualBasic, "Do`1")>]
    let ``Process project with types with correct fullname`` (name, sut: RoslynAnalyzer, project: Project) =
        let fullname = getTypeFullname name Seq.empty
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze project)
            |> (fun c -> c.Types)
            |> Seq.tryFind (fun c ->
                let typeFullname = getTypeFullname c.Identity.Name c.Identity.Parents
                typeFullname = fullname)

        test <@ typeDef.IsSome @>