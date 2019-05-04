namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_EnumModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (CSharp, ["
                enum NoAccessModifierType { }
            "])
            (VisualBasic, ["
                Enum NoAccessModifierType
                End Enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return enum with no access modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoAccessModifierType"
                        
                    test <@ result.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (CSharp, ["
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
            (VisualBasic, ["
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
                (withProjects contents, ("PublicType", Set ["public"]))
                (withProjects contents, ("InternalType", Set ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", Set ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", Set ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", Set ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", Set ["protected"; "internal"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.AccessModifiers |> Set
                                                   |> Set.map normalizeSyntax
                                                   |> (=) expected @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (CSharp, ["
                public enum NoModifierType { }
            "])
            (VisualBasic, ["
                Public enum NoModifierType
                End enum
            "])
        ]
        testList "Analyze/Enum" [
            yield! testRepeat (withProjects contents)
                "should return enum with no modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoModifierType"
                        
                    test <@ result.Modifiers |> Seq.isEmpty @>)
        ]