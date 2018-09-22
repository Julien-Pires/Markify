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
            yield! testRepeat (withProject "AccessModifiers") allLanguages [
                yield "should returns definition with no access modifier when type has none",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = TestHelper.filterTypes "NoAccessModifierType" assemblies
                        
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
                        let expected = TestHelper.getModifiers project.Language modifiers
                        let assemblies = sut.Analyze project
                        let result = TestHelper.filterTypes name assemblies
                        
                        test <@ result |> Seq.map (fun c -> Set c.Identity.AccessModifiers)
                                       |> Seq.forAllStrict ((Set.isSubset) (Set expected)) @>)
            ]

            yield! testRepeat (withProject "Modifiers") allLanguages [
                yield "should returns defintion with no modifier when type has none",
                fun project (sut : IProjectAnalyzer) ->
                    let assemblies = sut.Analyze project
                    let result = TestHelper.filterTypes "NoModifierType" assemblies
                        
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
                        let expected = TestHelper.getModifiers project.Language modifiers
                        let assemblies = sut.Analyze project
                        let result = TestHelper.filterTypes name assemblies
                        
                        test <@ result |> Seq.map (fun c -> Set c.Identity.Modifiers)
                                       |> Seq.forAllStrict ((Set.isSubset) (Set expected)) @>)
            ]
        ]