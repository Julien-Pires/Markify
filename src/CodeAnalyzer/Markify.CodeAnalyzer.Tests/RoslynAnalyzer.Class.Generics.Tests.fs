namespace Markify.CodeAnalyzer.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassGenerics_Tests =
    [<Tests>]
    let noGenericClassTests =
        let noGeneric = [
            (CSharp, ["
                public class NoGenericClass { }
            "])
            (VisualBasic, ["
                Public Class NoGenericClass
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects noGeneric) 
                "should return no generic parameters when class has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoGenericClass"

                    test <@ result.Generics |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let genericClassTests =
        let generic = [
            (CSharp, ["
                public class SingleGenericClass<T> { }
                public class MultipleGenericClass<T, Y> { }
            "])
            (VisualBasic, ["
                Public Class SingleGenericClass(Of T)
                End Class

                Public Class MultipleGenericClass(Of T, Y)
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized 
                "should return generic parameters when class has some" [
                (withProjects generic, ("SingleGenericClass`1", 1))
                (withProjects generic, ("MultipleGenericClass`2", 2))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.length = expected @>)

            yield! testRepeatParameterized 
                "should return valid generic parameter name when class has some" [
                (withProjects generic, ("SingleGenericClass`1", "T"))
                (withProjects generic, ("MultipleGenericClass`2", "T"))
                (withProjects generic, ("MultipleGenericClass`2", "Y"))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                        
                    test <@ result.Generics |> Seq.exists (fun c -> c.Name = expected) @>)
        ]
    
    [<Tests>]
    let genericModifersTests =
        let genericModifiers = [
            (CSharp, ["
                public class SingleGenericClass<T> { }
            "])
            (VisualBasic, ["
                Public Class SingleGenericClass(Of T)
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects genericModifiers)
                "should return no modifiers when class generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericClass`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Modifier.IsNone @>)
        ]

    [<Tests>]
    let genericConstraintsTests =
        let genericConstraints =[
            (CSharp, ["
                public class SingleGenericClass<T> { }
                public class GenericConstrainedClass<T, Y>
                    where T : struct
                    where Y : IEnumerable, class, new() { }
            "])
            (VisualBasic, ["
                Public Class SingleGenericClass(Of T)
                End Class
                Public Class GenericConstrainedClass(Of T As Structure, Y As { IEnumerable, Class, New })
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeat (withProjects genericConstraints) 
                "should return no constraints when class generic parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "SingleGenericClass`1"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = "T")

                    test <@ result.Constraints |> Seq.isEmpty @>)

            yield! testRepeatParameterized 
                "should return constraints when class generic parameter has some" [
                (withProjects genericConstraints, ("T", Set ["struct"]))
                (withProjects genericConstraints, ("Y", Set ["IEnumerable"; "class"; "new()"]))]
                (fun sut project (parameter, expected) () ->
                    let definition = sut.Analyze project |> findType "GenericConstrainedClass`2"
                    let result = definition.Generics |> Seq.find (fun c -> c.Name = parameter)

                    test <@ result.Constraints |> Set
                                               |> Set.map normalizeSyntax
                                               |> (=) expected @>)
        ]