module Roslyn_Processor_Process_Types_Modifiers_Tests
    open Markify.Roslyn

    open Markify.Models.IDE
    open Markify.Models.Definitions

    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "public", "PublicClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "internal", "InternalClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "protected", "ProtectedClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "private", "PrivateClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "public", "IPublicInterface")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "internal", "IInternalInterface")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "protected internal", "IProtectedInternalInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "public", "PublicStruct")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "internal", "InternalStruct")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalStruct")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "public", "PublicEnum")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "internal", "InternalEnum")>]
    [<ProjectData("EnumProject", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalEnum")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "public", "PublicDelegate")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "internal", "InternalDelegate")>]
    [<ProjectData("DelegateProject", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalDelegate")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Public", "PublicClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Friend", "InternalClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Protected", "ProtectedClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Protected Friend", "ProtectedInternalClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Private", "PrivateClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "Public", "IPublicInterface")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "Friend", "IInternalInterface")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "Protected Friend", "IProtectedInternalInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "Public", "PublicStruct")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "Friend", "InternalStruct")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "Protected Friend", "ProtectedInternalStruct")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "Public", "PublicEnum")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "Friend", "InternalEnum")>]
    [<ProjectData("EnumProject", ProjectLanguage.VisualBasic, "Protected Friend", "ProtectedInternalEnum")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "Public", "PublicDelegate")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "Friend", "InternalDelegate")>]
    [<ProjectData("DelegateProject", ProjectLanguage.VisualBasic, "Protected Friend", "ProtectedInternalDelegate")>]
    let ``Process project should return type access modifiers`` (modifier: string, name, sut: RoslynAnalyzer, project) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = 
            typeDef.AccessModifiers 
            |> Seq.filter (fun c -> Seq.contains c expectedModifiers) 

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "abstract", "AbstractClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "sealed", "SealedClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "partial", "PartialClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "static", "StaticClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.CSharp, "partial", "IPartialInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.CSharp, "partial", "PartialStruct")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "MustInherit", "AbstractClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "NotInheritable", "SealedClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Partial", "PartialClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "Static", "StaticClass")>]
    [<ProjectData("InterfaceProject", ProjectLanguage.VisualBasic, "Partial", "IPartialInterface")>]
    [<ProjectData("StructProject", ProjectLanguage.VisualBasic, "Partial", "PartialStruct")>]
    let ``Process project should return modifiers when type has one`` (modifier, name, sut: RoslynAnalyzer, project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.contains modifier typeDef.Modifiers @>

    [<Theory>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "abstract partial", "AbstractPartialClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.CSharp, "sealed partial", "SealedPartialClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "MustInherit Partial", "AbstractPartialClass")>]
    [<ProjectData("ClassProject", ProjectLanguage.VisualBasic, "NotInheritable Partial", "SealedPartialClass")>]
    let ``Process project should return all modifiers when type has multiple`` (modifier: string, name, sut: RoslynAnalyzer, project) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = Seq.filter (fun c -> Seq.contains c expectedModifiers) typeDef.Modifiers

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>