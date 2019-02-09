namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesModifiersTests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    struct NoAccessModifierType { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Structure NoAccessModifierType
                    End Structure
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return struct with no access modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public struct PublicType { }

                    internal struct InternalType { }

                    public partial struct ParentType
                    {
                        private struct PrivateType { }

                        protected struct ProtectedType { }

                        protected internal struct ProtectedInternalType { }

                        internal protected struct InternalProtectedType { }
                    }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
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
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized
                "should returns struct with access modifiers when type has some" [
                (withProjects contents, ("PublicType", ["public"]))
                (withProjects contents, ("InternalType", ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", ["protected"; "internal"]))]
                (fun sut project (name, modifiers) () ->
                    let expected = modifiers |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                        
                    test <@ Set result.Identity.AccessModifiers |> Set.isSubset (Set expected) @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public struct NoModifierType { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Structure NoModifierType
                    End Structure
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return struct with no modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withModifiersTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public abstract struct AbstractType { }

                    public partial struct PartialType { }

                    public sealed struct SealedType { }

                    public static struct StaticType { }

                    public abstract partial struct AbstractPartialType { }

                    public sealed partial struct SealedPartialType { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
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
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized
                "should returns struct with modifiers when type has some" [
                (withProjects contents, ("PartialType", ["partial"]))
                (withProjects contents, ("SealedType", ["sealed"]))
                (withProjects contents, ("AbstractType", ["abstract"]))
                (withProjects contents, ("StaticType", ["static"]))
                (withProjects contents, ("AbstractPartialType", ["abstract"; "partial"]))
                (withProjects contents, ("SealedPartialType", ["sealed"; "partial"]))]
                (fun sut project (name, modifiers) () ->
                    let expected = modifiers |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findStruct assemblies name
                        
                    test <@ Set result.Identity.Modifiers |> Set.isSubset (Set expected) @>)
        ]