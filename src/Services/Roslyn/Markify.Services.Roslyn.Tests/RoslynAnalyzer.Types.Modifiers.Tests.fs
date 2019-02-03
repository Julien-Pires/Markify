namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Ide
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesModifiersTests =
    [<Tests>]
    let analyzeTests =
        testList "Analyze" [
            yield! testRepeatOld (withProjectOld "AccessModifiers") allLanguages [
                yield "should returns definition with no access modifier when type has none",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies "NoAccessModifierType"
                        
                    test <@ result |> Seq.forAllStrict (fun c -> Seq.isEmpty c.Identity.AccessModifiers) @>

                yield! testTheory "should returns definition with access modifiers when type has some"
                    [("PublicType", ["public"]);
                    ("InternalType", ["internal"]);
                    ("ParentType.ProtectedType", ["protected"]);
                    ("ParentType.PrivateType", ["private"]);
                    ("ParentType.ProtectedInternalType", ["protected"; "internal"]);
                    ("ParentType.InternalProtectedType", ["protected"; "internal"]);]
                    (fun parameters project (sut : IProjectAnalyzer) -> 
                        let name, modifiers = parameters
                        let expected = modifiers |> List.map (fun c -> getModifier c project.Language)
                        let assemblies = sut.Analyze project
                        let result = filterTypes assemblies name
                        
                        test <@ result |> Seq.map (fun c -> Set c.Identity.AccessModifiers)
                                       |> Seq.forAllStrict ((Set.isSubset) (Set expected)) @>)
            ]

            yield! testRepeatOld (withProjectOld "Modifiers") allLanguages [
                yield "should returns defintion with no modifier when type has none",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = filterTypes assemblies "NoModifierType"
                        
                    test <@ result |> Seq.forAllStrict (fun c -> Seq.isEmpty c.Identity.Modifiers) @>

                yield! testTheory "should returns definition with modifiers when type has some"
                    [("PartialType", ["partial"]);
                    ("SealedType", ["sealed"]);
                    ("AbstractType", ["abstract"]);
                    ("StaticType", ["static"]);
                    ("AbstractPartialType", ["abstract"; "partial"]);
                    ("SealedPartialType", ["sealed"; "partial"])]
                    (fun parameters project (sut : IProjectAnalyzer) -> 
                        let name, modifiers = parameters
                        let expected = modifiers |> List.map (fun c -> getModifier c project.Language)
                        let assemblies = sut.Analyze project
                        let result = filterTypes assemblies name
                        
                        test <@ result |> Seq.map (fun c -> Set c.Identity.Modifiers)
                                       |> Seq.forAllStrict ((Set.isSubset) (Set expected)) @>)
            ]
        ]