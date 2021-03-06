﻿namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_DelegateModifiers_Tests =
    [<Tests>]
    let noAccessModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                delegate void NoAccessModifierType();
            "])
            (ProjectLanguage.VisualBasic, ["
                Delegate Sub NoAccessModifierType()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects contents)
                "should return delegate with no access modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "NoAccessModifierType"
                        
                    test <@ result.Identity.AccessModifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withAccessModifiersTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public delegate void PublicType();
                internal delegate void InternalType();
                public partial class ParentType
                {
                    private delegate void PrivateType();
                    protected delegate void ProtectedType();
                    protected internal delegate void ProtectedInternalType();
                    internal protected delegate void InternalProtectedType();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub PublicType()
                Friend Delegate Sub InternalType()
                Partial Public Class ParentType
                    Private Delegate Sub PrivateType()
                    Protected Delegate Sub ProtectedType()
                    Protected Friend Delegate Sub ProtectedInternalType()
                    Protected Friend Delegate Sub InternalProtectedType()
                End Class
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeatParameterized
                "should returns delegate with access modifiers when type has some" [
                (withProjects contents, ("PublicType", Set ["public"]))
                (withProjects contents, ("InternalType", Set ["internal"]))
                (withProjects contents, ("ParentType.ProtectedType", Set ["protected"]))
                (withProjects contents, ("ParentType.PrivateType", Set ["private"]))
                (withProjects contents, ("ParentType.ProtectedInternalType", Set ["protected"; "internal"]))
                (withProjects contents, ("ParentType.InternalProtectedType", Set ["protected"; "internal"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findDelegate name
                        
                    test <@ result.Identity.AccessModifiers |> Set
                                                            |> Set.map normalizeSyntax
                                                            |> (=) expected @>)
        ]

    [<Tests>]
    let noModifierTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public delegate void NoModifierType();
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Delegate Sub NoModifierType()
            "])
        ]
        testList "Analyze/Delegate" [
            yield! testRepeat (withProjects contents)
                "should return delegate with no modifier when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findDelegate "NoModifierType"
                        
                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]