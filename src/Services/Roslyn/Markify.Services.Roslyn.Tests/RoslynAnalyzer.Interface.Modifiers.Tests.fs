namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
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
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
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
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                        
                    test <@ result.Identity.AccessModifiers |> Seq.map normalizeSyntax
                                                            |> Set
                                                            |> Set.isSubset expected @>)
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
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
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
            yield! testRepeatParameterized
                "should returns interface with modifiers when type has some" [
                (withProjects contents, ("PartialType", Set ["partial"]))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findInterface assemblies name
                        
                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax
                                                      |> Set
                                                      |> Set.isSubset expected @>)
        ]