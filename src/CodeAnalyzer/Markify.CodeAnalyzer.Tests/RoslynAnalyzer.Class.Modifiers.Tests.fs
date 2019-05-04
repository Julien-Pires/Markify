namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (CSharp, ["
                class NoAccessModifierType { }
            "])
            (VisualBasic, ["
                Class NoAccessModifierType
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects contents)
                "should return class with no access modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoAccessModifierType"
                        
                    test <@ result.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (CSharp, ["
                public class PublicType { }
                internal class InternalType { }
                public partial class ParentType
                {
                    private class PrivateType { }
                    protected class ProtectedType { }
                    protected internal class ProtectedInternalType { }
                    internal protected class InternalProtectedType { }
                }
            "])
            (VisualBasic, ["
                Public Class PublicType
                End Class
                Friend Class InternalType
                End Class
                Partial Public Class ParentType
                    Private Class PrivateType
                    End Class
                    Protected Class ProtectedType
                    End Class
                    Protected Friend Class ProtectedInternalType
                    End Class
                    Protected Friend Class InternalProtectedType
                    End Class
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should returns class with access modifiers when type has some" [
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
                public class NoModifierType { }
            "])
            (VisualBasic, ["
                Public Class NoModifierType
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects contents)
                "should return class with no modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoModifierType"
                        
                    test <@ result.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withModifiersTests =
        let contents = [
            (CSharp, ["
                public abstract class AbstractType { }
                public partial class PartialType { }
                public sealed class SealedType { }
                public static class StaticType { }
                public abstract partial class AbstractPartialType { }
                public sealed partial class SealedPartialType { }
            "])
            (VisualBasic, ["
                Public MustInherit Class AbstractType
                End Class
                Partial Public Class PartialType
                End Class
                Public NotInheritable Class SealedType
                End Class
                Public Static Class StaticType
                End Class
                Partial Public MustInherit Class AbstractPartialType
                End Class
                Partial Public NotInheritable Class SealedPartialType
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should returns class with modifiers when type has some" [
                (withProjects contents, ("PartialType", Set ["partial"]))
                (withProjects contents, ("SealedType", Set ["sealed"]))
                (withProjects contents, ("AbstractType", Set ["abstract"]))
                (withProjects contents, ("StaticType", Set ["static"]))
                (withProjects contents, ("AbstractPartialType", Set ["abstract"; "partial"]))
                (withProjects contents, ("SealedPartialType", Set ["sealed"; "partial"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Modifiers |> Set
                                             |> Set.map normalizeSyntax
                                             |> (=) expected @>)
        ]