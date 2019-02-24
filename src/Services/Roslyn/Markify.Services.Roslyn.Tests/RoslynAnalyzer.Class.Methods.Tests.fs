namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassMethods_Tests =
    [<Tests>]
    let noMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class WithoutMethods {}
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class WithoutMethods
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no method when class has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies "WithoutMethods"

                    test <@ result.Methods |> Seq.isEmpty @>)
        ]
    
    [<Tests>]
    let withMethodsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class SingleMethods 
                {
                    void FirstMethod() { }
                }
                public class MultipleMethods 
                {
                    void FirstMethod() { }
                    void SecondMethod() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class SingleMethods
                    Sub FirstMethod()
                    End Sub
                End Class
                Public Class MultipleMethods
                    Sub FirstMethod()
                    End Sub
                    Sub SecondMethod()
                    End Sub
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return methods when class has some" [
                (withProjects content, ("SingleMethods", 1))
                (withProjects content, ("MultipleMethods", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Methods |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct class method name" [
                (withProjects content, ("SingleMethods", "FirstMethod"))
                (withProjects content, ("MultipleMethods", "SecondMethod"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Methods |> Seq.exists (fun c -> c.Identity.Name = expected) @>)
        ]

    [<Tests>]
    let acessModifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class AccessModifiers 
                {
                    void WithoutAccessModifier() { }
                    public void PublicMethod() { }
                    internal void InternalMethod() { }
                    protected void ProtectedMethod() { }
                    protected internal void ProtectedInternalMethod() { }
                    private void PrivateMethod() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class AccessModifiers
                    Sub WithoutAccessModifier()
                    End Sub
                    Public Sub PublicMethod()
                    End Sub
                    Friend Sub InternalMethod()
                    End Sub
                    Protected Sub ProtectedMethod()
                    End Sub
                    Protected Friend Sub ProtectedInternalMethod()
                    End Sub
                    Private Sub PrivateMethod()
                    End Sub
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return correct class method access modifier" [
                (withProjects content, ("WithoutAccessModifier", Set ["private"]))
                (withProjects content, ("PublicMethod", Set ["public"]))
                (withProjects content, ("InternalMethod", Set ["internal"]))
                (withProjects content, ("ProtectedMethod", Set ["protected"]))
                (withProjects content, ("ProtectedInternalMethod", Set ["protected"; "internal"]))
                (withProjects content, ("PrivateMethod", Set ["private"]))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "AccessModifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.AccessModifiers |> Seq.map normalizeSyntax 
                                                            |> Set
                                                            |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let modifiersTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class Modifiers 
                {
                    void WithoutModifier()
                    static void StaticMethod() { }
                    virtual void VirtualMethod() { }
                    sealed void SealedMethod() { }
                    override void OverrideMethod() { }
                    partial void PartialMethod();
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class Modifiers
                    Sub WithoutModifier()
                    End Sub
                    Shared Sub StaticMethod()
                    End Sub
                    Overridable Sub VirtualMethod()
                    End Sub
                    NotOverridable Sub SealedMethod()
                    End Sub
                    Overrides Sub OverrideMethod()
                    End Sub
                    Partial Sub PartialMethod()
                    End Sub
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no modifiers when class method has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Modifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutModifier")

                    test <@ result.Identity.Modifiers |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return correct modifier when class method has one" [
                (withProjects content, ("StaticMethod", Set ["static"]))
                (withProjects content, ("VirtualMethod", Set ["virtual"]))
                (withProjects content, ("SealedMethod", Set ["sealed"]))
                (withProjects content, ("OverrideMethod", Set ["override"]))
                (withProjects content, ("PartialMethod", Set ["partial"]))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Modifiers"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Modifiers |> Seq.map normalizeSyntax 
                                                      |> Set
                                                      |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let genericTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class Generics 
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
                Public Class Generics
                    Sub WithoutGenerics
                    End Sub
                    Sub SingleGeneric(Of T)()
                    End Sub
                    Sub MultipleGeneric(Of T As Int32, Y As {Int32, Class})()
                    End Sub
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no generic parameters when class method has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutGenerics")

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic parameters when class method has some" [
                (withProjects content, ("SingleGeneric", 1))
                (withProjects content, ("MultipleGeneric", 2))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeat (withProjects content)
                "should return no generic constraint when class method generic parameter has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleGeneric")

                    test <@ result.Identity.Parameters |> Seq.map (fun c -> c.Constraints)
                                                       |> Seq.forall Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return generic constraint when class method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", 1))
                (withProjects content, ("MultipleGeneric", "Y", 2))]
                (fun sut project (method, parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Generics"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct generic constraint name when class method generic parameter has some" [
                (withProjects content, ("MultipleGeneric", "T", "Int32"))
                (withProjects content, ("MultipleGeneric", "Y", "class"))]
                (fun sut project (method, parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Generics"
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
                public class Parameters 
                {
                    void WithoutParameters() { }
                    void SingleParameters(Int32 A) { }
                    void MultipleParameters(ref Int32 A = 32, out Single B = 16) { }
                    void GenericParameters<T>(T A) { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class Parameters
                    Sub WithoutParameters
                    End Sub
                    Sub SingleParameters(A As Int32)
                    End Sub
                    Sub MultipleParameters(ByRef A As Int32 = 32, ByVal B As Single = 16)
                    End Sub
                    Sub GenericParameters(Of T)(A As T)
                    End Sub
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects content)
                "should return no parameters when class method has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "WithoutParameters")

                    test <@ result.Parameters |> Seq.isEmpty @>)

            yield! testRepeatParameterized
                "should return parameters when class method has some" [
                (withProjects content, ("SingleParameters", 1))
                (withProjects content, ("MultipleParameters", 2))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameter name for class method" [
                (withProjects content, ("SingleParameters", "A"))
                (withProjects content, ("MultipleParameters", "B"))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)

            yield! testRepeatParameterized
                "should return correct parameter type for class method" [
                (withProjects content, ("SingleParameters", "A", "Int32"))
                (withProjects content, ("MultipleParameters", "B", "Single"))
                (withProjects content, ("GenericParameters", "A", "T"))]
                (fun sut project (method, parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.Type = expected @>)

            yield! testRepeat (withProjects content)
                "should return no modifier when class method parameter has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.Modifier.IsNone @>)

            yield! testRepeatParameterized
                "should return modifier when class method parameter has one" [
                (withProjects content, ("A", "ref"))
                (withProjects content, ("B", "out"))]
                (fun sut project (parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> normalizeSyntax c.Modifier.Value = expected @>)

            yield! testRepeat (withProjects content)
                "should return no default value when class method parameter has none"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "SingleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = "A")
                                              |> fun c -> c.DefaultValue.IsNone @>)

            yield! testRepeatParameterized
                "should return default value when class method parameter has one" [
                (withProjects content, ("A", "32"))
                (withProjects content, ("B", "16"))]
                (fun sut project (parameter, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Parameters"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = "MultipleParameters")

                    test <@ result.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                              |> fun c -> c.DefaultValue = Some expected @>)
        ]
    
    [<Tests>]
    let returnTypeTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class ReturnType 
                {
                    void Method() { }
                    Int32 Function() { }
                    T GenericFunction<T>() { }
                }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class ReturnType
                    Sub Method()
                    End Sub
                    Function Function() As Int32
                    End Function
                    Function GenericFunction(Of T)() As T
                    End Function
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return correct return type for class method" [
                (withProjects content, ("Method", "void"))
                (withProjects content, ("Function", "Int32"))
                (withProjects content, ("GenericFunction", "T"))]
                (fun sut project (method, expected) () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "ReturnType"
                    let result = object.Methods |> Seq.find (fun c -> c.Identity.Name = method)

                    test <@ result.ReturnType |> normalizeSyntax = expected @>)
        ]