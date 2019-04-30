namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_CommentsParameters_Tests =
    [<Tests>]
    let noParameterTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                /// <summary></summary>
                public class NoParameter { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary></summary>
                Public Class NoParameter
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeat (withProjects contents)
                "should return no parameter when comment has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "NoParameter"
                    let result = definition.Comments |> Seq.find (fun c -> c.Name = "summary")
                    
                    test <@ result.Parameter |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withParameterTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                /// <summary name=''></summary>
                /// <remarks name='' value='' data></remarks>
                public class WithParameters { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary name=''></summary>
                ''' <remarks name='' value='' data></remarks>
                Public Class WithParameters
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeatParameterized
                "should return parameters when comment has some" [
                (withProjects contents, ("summary", 1))
                (withProjects contents, ("remarks", 3))]
                (fun sut project (comment, expected) () ->
                    let definition = sut.Analyze project |> findType "WithParameters"
                    let result = definition.Comments |> Seq.find (fun c -> c.Name = comment)
                    
                    test <@ result.Parameter |> Seq.length = expected @>)

            yield! testRepeatParameterized
                "should return correct parameter name when comment has some" [
                (withProjects contents, ("summary", Set ["name"]))
                (withProjects contents, ("remarks", Set ["name"; "value"; "data"]))]
                (fun sut project (comment, expected) () ->
                    let definition = sut.Analyze project |> findType "WithParameters"
                    let result = definition.Comments |> Seq.find (fun c -> c.Name = comment)
                    
                    test <@ result.Parameter |> Seq.map (fun c -> c.Name)
                                             |> Set
                                             |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let noParameterValueTests = 
        let contents = [
            (ProjectLanguage.CSharp, ["
                /// <summary name></summary>
                public class NoValue { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary name></summary>
                Public Class NoValue
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeat (withProjects contents)
                "should return no value when comment parameter has none"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "NoValue"
                    let result = definition.Comments |> Seq.find (fun c -> c.Name = "summary")
                    
                    test <@ result.Parameter |> Seq.find (fun c -> c.Name = "name")
                                             |> fun c -> c.Value.IsNone @>)
        ]

    [<Tests>]
    let withParameterValueTests = 
        let contents = [
            (ProjectLanguage.CSharp, ["
                /// <summary name='foo'></summary>
                public class WithValue { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary name='foo'></summary>
                Public Class WithValue
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeat (withProjects contents)
                "should return value when comment parameter has one"
                (fun sut project () ->
                    let definition = sut.Analyze project |> findType "WithValue"
                    let result = definition.Comments |> Seq.find (fun c -> c.Name = "summary")
                    
                    test <@ result.Parameter |> Seq.find (fun c -> c.Name = "name")
                                             |> fun c -> c.Value = Some "foo" @>)
        ]