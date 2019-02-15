namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_EnumModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                enum NoAccessModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Enum NoAccessModifierType
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return enum with no access modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public enum PublicType { }
                internal enum InternalType { }
                public partial class ParentType
                {
                    private enum PrivateType { }
                    protected enum ProtectedType { }
                    protected internal enum ProtectedInternalType { }
                    internal protected enum InternalProtectedType { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Enum PublicType
                End Enum
                Friend Enum InternalType
                End Enum
                Partial Public Class ParentType
                    Private Enum PrivateType
                    End Enum
                    Protected Enum ProtectedType
                    End Enum
                    Protected Friend Enum ProtectedInternalType
                    End Enum
                    Protected Friend Enum InternalProtectedType
                    End Enum
                End Class
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeatParameterized
                "should returns enum with access modifiers when type has some" [
                (withProjects contents, ("PublicType", ["public"]))
                (withProjects contents, ("InternalType", ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", ["protected"; "internal"]))]
                (fun sut project (name, modifiers) () ->
                    let expected = modifiers |> List.map normalizeSyntax |> Set
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies name
                        
                    test <@ Set result.Identity.AccessModifiers |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public enum NoModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public enum NoModifierType
                End enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return enum with no modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findEnum assemblies "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]