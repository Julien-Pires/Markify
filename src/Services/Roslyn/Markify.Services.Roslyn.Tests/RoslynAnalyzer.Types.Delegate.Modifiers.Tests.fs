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
                    delegate void NoAccessModifierType();
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Delegate Sub NoAccessModifierType()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return delegate with no access modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void PublicType();

                    internal delegate void InternalType();

                    public partial class ParentType
                    {
                        private delegate void PrivateType();

                        protected delegate void ProtectedType();

                        protected internal delegate void ProtectedInternalType();

                        internal protected delegate void InternalProtectedType();
                    }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub PublicType()

                    Friend Delegate Sub InternalType()

                    Partial Public Class ParentType
                        Private Delegate Sub PrivateType()

                        Protected Delegate Sub ProtectedType()

                        Protected Friend Delegate Sub ProtectedInternalType()

                        Protected Friend Delegate Sub InternalProtectedType()
                    End Class
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized
                "should returns delegate with access modifiers when type has some" [
                (withProjects contents, ("PublicType", ["public"]))
                (withProjects contents, ("InternalType", ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", ["protected"; "internal"]))]
                (fun sut project (name, modifiers) () ->
                    let expected = modifiers |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies name
                        
                    test <@ Set result.Identity.AccessModifiers |> Set.isSubset (Set expected) @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public delegate void NoModifierType();
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Delegate Sub NoModifierType()
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeat (withProjects contents)
                "should return delegate with no modifier when type has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findDelegate assemblies "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]