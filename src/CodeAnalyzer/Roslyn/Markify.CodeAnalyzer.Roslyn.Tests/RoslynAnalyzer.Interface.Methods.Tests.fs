namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_InterfaceMethods_Tests =
    [<Tests>]
    let noMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface WithoutMethods {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface WithoutMethods
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no method when interface has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findInterface "WithoutMethods"

                    test <@ result.Methods |> Seq.isEmpty @>)
        ]
    
    [<Tests>]
    let withMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface SingleMethods 
                {
                    void FirstMethod();
                }
                public interface MultipleMethods 
                {
                    void FirstMethod();
                    void SecondMethod();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface SingleMethods
                    Sub FirstMethod()
                End Interface
                Public Interface MultipleMethods
                    Sub FirstMethod()
                    Sub SecondMethod()
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return methods when interface has some" [
                (withProjects content, ("SingleMethods", 1))
                (withProjects content, ("MultipleMethods", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findInterface name

                    test <@ result.Methods |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct interface method name" [
                (withProjects content, ("SingleMethods", "FirstMethod"))
                (withProjects content, ("MultipleMethods", "SecondMethod"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findInterface name

                    test <@ result.Methods |> Seq.exists (fun c -> c.Identity.Name = expected) @>)
        ]

    [<Tests>]
    let acessModifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface AccessModifiers 
                {
                    void WithoutAccessModifier();
                    public void PublicMethod();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface AccessModifiers
                    Sub WithoutAccessModifier()
                    Public Sub PublicMethod()
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return correct interface method access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["public"]))
                (withProjects content, ("PublicMethod", Set ["public"]))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findInterface "AccessModifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.AccessModifiers |> Set
                                                            |> Set.map normalizeSyntax 
                                                            |> (=) expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface Modifiers 
                {
                    void WithoutModifier()
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface Modifiers
                    Sub WithoutModifier()
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no modifiers when interface method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Modifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutModifier")

                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface Generics 
                {
                    void WithoutGenerics();
                    void SingleGeneric<T>();
                    void MultipleGeneric<T, Y>()
                        where T : Int32
                        where Y : Int32, class
                   ;
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface Generics
                    Sub WithoutGenerics
                    Sub SingleGeneric(Of T)()
                    Sub MultipleGeneric(Of T As Int32, Y As {Int32, Class})()
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no generic parameters when interface method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutGenerics")

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic parameters when interface method has some" [
                (withProjects content, ("SingleGeneric", 1))
                (withProjects content, ("MultipleGeneric", 2))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findInterface "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeat (withProjects content)
                "should return no generic constraint when interface method generic parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleGeneric")

                    test <@ result.Identity.Parameters |> Seq.map (fun c -> c.Constraints)
                                                       |> Seq.forall Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic constraint when interface method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", 1))
                (withProjects content, ("MultipleGeneric", "Y", 2))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findInterface "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct generic constraint name when interface method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", "Int32"))
                (withProjects content, ("MultipleGeneric", "Y", "class"))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findInterface "Generics"
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
                public interface Parameters 
                {
                    void WithoutParameters();
                    void SingleParameters(Int32 A);
                    void MultipleParameters(ref Int32 A = 32, out Single B = 16);
                    void GenericParameters<T>(T A);
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface Parameters
                    Sub WithoutParameters
                    Sub SingleParameters(A As Int32)
                    Sub MultipleParameters(ByRef A As Int32 = 32, ByVal B As Single = 16)
                    Sub GenericParameters(Of T)(A As T)
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeat (withProjects content)
                "should return no parameters when interface method has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutParameters")

                    test <@ result.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return parameters when interface method has some" [
                (withProjects content, ("SingleParameters", 1))
                (withProjects content, ("MultipleParameters", 2))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameter name for interface method" [
                (withProjects content, ("SingleParameters", "A"))
                (withProjects content, ("MultipleParameters", "B"))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |>  findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct parameter type for interface method" [
                (withProjects content, ("SingleParameters", "A", "Int32"))
                (withProjects content, ("MultipleParameters", "B", "Single"))
                (withProjects content, ("GenericParameters", "A", "T"))]
                (fun sut project (method, parameter, expected) () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Type = expected @>)

            yield! testRepeat (withProjects content)
                "should return no modifier when interface method parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.Modifier.IsNone @>)

            yield! testRepeatParameterized
                "should return modifier when interface method parameter has one" [
                (withProjects content, ("A", "ref"))
                (withProjects content, ("B", "out"))]
                (fun sut project (parameter, expected) () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> normalizeSyntax c.Modifier.Value = expected @>)

            yield! testRepeat (withProjects content)
                "should return no default value when interface method parameter has none"
                (fun sut project () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeatParameterized
                "should return default value when interface method parameter has one" [
                (withProjects content, ("A", "32"))
                (withProjects content, ("B", "16"))]
                (fun sut project (parameter, expected) () ->
                    let object = sut.Analyze project |> findInterface "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.DefaultValue = Some expected @>)
        ]
    
    [<Tests>]
    let returnTypeTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public interface ReturnType 
                {
                    void Method();
                    Int32 Function();
                    T GenericFunction<T>();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Interface ReturnType
                    Sub Method()
                    Function Function() As Int32
                    Function GenericFunction(Of T)() As T
                End Interface
            "])
        ]
        testList "Analyze/Interface" [
            yield! testRepeatParameterized
                "should return correct return type for interface method" [
                (withProjects content, ("Method", "void"))
                (withProjects content, ("Function", "Int32"))
                (withProjects content, ("GenericFunction", "T"))]
                (fun sut project (method, expected) () ->
                    let object = sut.Analyze project |> findInterface "ReturnType"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.ReturnType |> normalizeSyntax = expected @>)
        ]