namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_StructMethods_Tests =
    [<Tests>]
    let noMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct WithoutMethods {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure WithoutMethods
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no method when struct has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findStruct "WithoutMethods"

                    test <@ result.Methods |> Seq.isEmpty @>)
        ]
    
    [<Tests>]
    let withMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct SingleMethods 
                {
                    void FirstMethod() { }
                }
                public struct MultipleMethods 
                {
                    void FirstMethod() { }
                    void SecondMethod() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure SingleMethods
                    Sub FirstMethod()
                    End Sub
                End Structure
                Public Structure MultipleMethods
                    Sub FirstMethod()
                    End Sub
                    Sub SecondMethod()
                    End Sub
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return methods when struct has some" [
                (withProjects content, ("SingleMethods", 1))
                (withProjects content, ("MultipleMethods", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Methods |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct struct method name" [
                (withProjects content, ("SingleMethods", "FirstMethod"))
                (withProjects content, ("MultipleMethods", "SecondMethod"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findStruct name

                    test <@ result.Methods |> Seq.exists (fun c -> c.Identity.Name = expected) @>)
        ]

    [<Tests>]
    let acessModifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct AccessModifiers 
                {
                    void WithoutAccessModifier() { }
                    public void PublicMethod() { }
                    internal void InternalMethod() { }
                    private void PrivateMethod() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure AccessModifiers
                    Sub WithoutAccessModifier()
                    End Sub
                    Public Sub PublicMethod()
                    End Sub
                    Friend Sub InternalMethod()
                    End Sub
                    Private Sub PrivateMethod()
                    End Sub
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return correct struct method access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicMethod", Set ["public"]))
                (withProjects content, ("InternalMethod", Set ["internal"]))
                (withProjects content, ("PrivateMethod", Set ["private"]))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "AccessModifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.AccessModifiers |> Seq.map normalizeSyntax 
                                                            |> Set
                                                            |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct Modifiers 
                {
                    void WithoutModifier()
                    static void StaticMethod() { }
                    partial void PartialMethod();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure Modifiers
                    Sub WithoutModifier()
                    End Sub
                    Shared Sub StaticMethod()
                    End Sub
                    Partial Sub PartialMethod()
                    End Sub
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no modifiers when struct method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Modifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutModifier")

                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return correct modifier when struct method has one" [
                (withProjects content, ("StaticMethod", Set ["static"]))
                (withProjects content, ("PartialMethod", Set ["partial"]))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "Modifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax 
                                                      |> Set
                                                      |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let genericTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct Generics 
                {
                    void WithoutGenerics() { }
                    void SingleGeneric<T>() { }
                    void MultipleGeneric<T, Y>()
                        where T : Int32
                        where Y : Int32, class
                    { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure Generics
                    Sub WithoutGenerics
                    End Sub
                    Sub SingleGeneric(Of T)()
                    End Sub
                    Sub MultipleGeneric(Of T As Int32, Y As {Int32, Class})()
                    End Sub
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no generic parameters when struct method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutGenerics")

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic parameters when struct method has some" [
                (withProjects content, ("SingleGeneric", 1))
                (withProjects content, ("MultipleGeneric", 2))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeat (withProjects content)
                "should return no generic constraint when struct method generic parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleGeneric")

                    test <@ result.Identity.Parameters |> Seq.map (fun c -> c.Constraints)
                                                       |> Seq.forall Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic constraint when struct method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", 1))
                (withProjects content, ("MultipleGeneric", "Y", 2))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct generic constraint name when struct method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", "Int32"))
                (withProjects content, ("MultipleGeneric", "Y", "class"))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints
                                                       |> Seq.map normalizeSyntax 
                                                       |> Seq.contains expected @>)
        ]

    [<Tests>]
    let parametersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct Parameters 
                {
                    void WithoutParameters() { }
                    void SingleParameters(Int32 A) { }
                    void MultipleParameters(ref Int32 A = 32, out Single B = 16) { }
                    void GenericParameters<T>(T A) { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure Parameters
                    Sub WithoutParameters
                    End Sub
                    Sub SingleParameters(A As Int32)
                    End Sub
                    Sub MultipleParameters(ByRef A As Int32 = 32, ByVal B As Single = 16)
                    End Sub
                    Sub GenericParameters(Of T)(A As T)
                    End Sub
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeat (withProjects content)
                "should return no parameters when struct method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutParameters")

                    test <@ result.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return parameters when struct method has some" [
                (withProjects content, ("SingleParameters", 1))
                (withProjects content, ("MultipleParameters", 2))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameter name for struct method" [
                (withProjects content, ("SingleParameters", "A"))
                (withProjects content, ("MultipleParameters", "B"))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct parameter type for struct method" [
                (withProjects content, ("SingleParameters", "A", "Int32"))
                (withProjects content, ("MultipleParameters", "B", "Single"))
                (withProjects content, ("GenericParameters", "A", "T"))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Type = expected @>)

            yield! testRepeat (withProjects content)
                "should return no modifier when struct method parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.Modifier.IsNone @>)

            yield! testRepeatParameterized
                "should return modifier when struct method parameter has one" [
                (withProjects content, ("A", "ref"))
                (withProjects content, ("B", "out"))]
                (fun sut project (parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> normalizeSyntax c.Modifier.Value = expected @>)

            yield! testRepeat (withProjects content)
                "should return no default value when struct method parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeatParameterized
                "should return default value when struct method parameter has one" [
                (withProjects content, ("A", "32"))
                (withProjects content, ("B", "16"))]
                (fun sut project (parameter, expected) () ->
                    let object = sut.Analyze project |> findStruct "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.DefaultValue = Some expected @>)
        ]
    
    [<Tests>]
    let returnTypeTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public struct ReturnType 
                {
                    void Method() { }
                    Int32 Function() { }
                    T GenericFunction<T>() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Structure ReturnType
                    Sub Method()
                    End Sub
                    Function Function() As Int32
                    End Function
                    Function GenericFunction(Of T)() As T
                    End Function
                End Structure
            "])
        ]
        testList "Analyze/Struct" [
            yield! testRepeatParameterized
                "should return correct return type for struct method" [
                (withProjects content, ("Method", "void"))
                (withProjects content, ("Function", "Int32"))
                (withProjects content, ("GenericFunction", "T"))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findStruct "ReturnType"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.ReturnType |> normalizeSyntax = expected @>)
        ]