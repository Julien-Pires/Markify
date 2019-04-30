namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                interface NoAccessModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Interface NoAccessModifierType
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects contents)
                "should return interface with no access modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoAccessModifierType"
                        
                    test <@ result.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public interface PublicType { }
                internal interface InternalType { }
                public partial interface ParentType
                {
                    private interface PrivateType { }
                    protected interface ProtectedType { }
                    protected internal interface ProtectedInternalType { }
                    internal protected interface InternalProtectedType { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface PublicType
                End Interface
                Friend Interface InternalType
                End Interface
                Partial Public Interface ParentType
                    Private Interface PrivateType
                    End Interface
                    Protected Interface ProtectedType
                    End Interface
                    Protected Friend Interface ProtectedInternalType
                    End Interface
                    Protected Friend Interface InternalProtectedType
                    End Interface
                End Class
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should returns interface with access modifiers when type has some" [
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
            (ProjectLanguage.CSharp, ["
                public interface NoModifierType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface NoModifierType
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects contents)
                "should return interface with no modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoModifierType"
                        
                    test <@ result.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public partial interface PartialType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Partial Public Interface PartialType
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects contents)
                "should returns interface with modifiers when type has some"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "PartialType"
                        
                    test <@ result.Modifiers |> Set
                                             |> Set.map normalizeSyntax
                                             |> (=) (Set ["partial"]) @>)
        ]