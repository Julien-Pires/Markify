namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                struct NoAccessModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Structure NoAccessModifierType
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects contents)
                "should return struct with no access modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public struct PublicType { }
                internal struct InternalType { }
                public partial struct ParentType
                {
                    private struct PrivateType { }
                    protected struct ProtectedType { }
                    protected internal struct ProtectedInternalType { }
                    internal protected struct InternalProtectedType { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure PublicType
                End Structure
                Friend Structure InternalType
                End Structure
                Partial Public Structure ParentType
                    Private Structure PrivateType
                    End Structure
                    Protected Structure ProtectedType
                    End Structure
                    Protected Friend Structure ProtectedInternalType
                    End Structure
                    Protected Friend Structure InternalProtectedType
                    End Structure
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should returns struct with access modifiers when type has some" [
                (withProjects contents, ("PublicType", Set ["public"]))
                (withProjects contents, ("InternalType", Set ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", Set ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", Set ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", Set ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", Set ["protected"; "internal"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name
                        
                    test <@ result.Identity.AccessModifiers |> Set
                                                            |> Set.map normalizeSyntax
                                                            |> (=) expected @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public struct NoModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure NoModifierType
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects contents)
                "should return struct with no modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public abstract struct AbstractType { }
                public partial struct PartialType { }
                public sealed struct SealedType { }
                public static struct StaticType { }
                public abstract partial struct AbstractPartialType { }
                public sealed partial struct SealedPartialType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public MustInherit Structure AbstractType
                End Structure
                Partial Public Structure PartialType
                End Structure
                Public NotInheritable Structure SealedType
                End Structure
                Public Static Structure StaticType
                End Structure
                Partial Public MustInherit Structure AbstractPartialType
                End Structure
                Partial Public NotInheritable Structure SealedPartialType
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should returns struct with modifiers when type has some" [
                (withProjects contents, ("PartialType", Set ["partial"]))
                (withProjects contents, ("SealedType", Set ["sealed"]))
                (withProjects contents, ("AbstractType", Set ["abstract"]))
                (withProjects contents, ("StaticType", Set ["static"]))
                (withProjects contents, ("AbstractPartialType", Set ["abstract"; "partial"]))
                (withProjects contents, ("SealedPartialType", Set ["sealed"; "partial"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name
                        
                    test <@ result.Identity.Modifiers |> Set
                                                      |> Set.map normalizeSyntax
                                                      |> (=) expected @>)
        ]