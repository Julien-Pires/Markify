namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassGenerics_Tests =
    [<Tests>]
    let noGenericClassTests =
        let noGeneric = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class NoGenericClass { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Class NoGenericClass
                    End Class
                "]
            )
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized 
                "should return no generic parameters when class has none" [
                (withProjects noGeneric, "NoGenericClass")]
                (fun sut project name () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Identity.Parameters |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericClassTests =
        let generic = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class SingleGenericClass<T> { }

                    public class MultipleGenericClass<T, Y> { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Class SingleGenericClass(Of T)
                    End Class

                    Public Class MultipleGenericClass(Of T, Y)
                    End Class
                "]
            )
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized 
                "should return generic parameters when class has some" [
                (withProjects generic, ("SingleGenericClass`1", 1))
                (withProjects generic, ("MultipleGenericClass`2", 2))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name 
                        
                    test <@ result.Identity.Parameters |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when class has some" [
                (withProjects generic, ("SingleGenericClass`1", "T"))
                (withProjects generic, ("MultipleGenericClass`2", "T"))
                (withProjects generic, ("MultipleGenericClass`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name
                        
                    test <@ result.Identity.Parameters |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class SingleGenericClass<T> { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Class SingleGenericClass(Of T)
                    End Class
                "]
            )
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized 
                "should return no modifiers when class generic parameter has none" [
                (withProjects genericModifiers, ("SingleGenericClass`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Modifier.IsNone @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (
                ProjectLanguage.CSharp,
                ["
                    public class SingleGenericClass<T> { }

                    public class GenericConstrainedClass<T, Y>
                        where T : struct
                        where Y : IEnumerable, class, new() { }
                "]
            )
            (
                ProjectLanguage.VisualBasic,
                ["
                    Public Class SingleGenericClass(Of T)
                    End Class

                    Public Class GenericConstrainedClass(Of T As Structure, Y As { IEnumerable, Class, New })
                    End Class
                "]
            )
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized 
                "should return no constraints when class generic parameter has none" [
                (withProjects genericConstraints, ("SingleGenericClass`1", "T"))]
                (fun sut project (name, parameter) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints
                                                       |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when class generic parameter has some" [
                (withProjects genericConstraints, ("GenericConstrainedClass`2", "T", ["struct"]))
                (withProjects genericConstraints, ("GenericConstrainedClass`2", "Y", ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (name, parameter, constraints) () ->
                    let expected = constraints |> List.map normalizeSyntax
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name

                    test <@ result.Identity.Parameters |> Seq.find (fun c -> c.Name = parameter)
                                                       |> fun c -> c.Constraints |> Seq.toList = expected @>)
        ]