namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_Comments_Tests =
    [<Tests>]
    let noCommentTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                public class NoCommentType { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class NoCommentType
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeat (withProjects content)
                "should return no comment when type has none"
                (fun sut project () ->
                    let result = sut.Analyze project |> findType "NoCommentType"
                    
                    test <@ result.Comments |> Seq.isEmpty @>)
        ]

    [<Tests>]
    let withCommentsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                /// <summary></summary>
                public class SingleComment { }
                /// <summary></summary>
                /// <remarks></remarks>
                public class MultipleComment { }
                /// <inheritdoc />
                public class SelfClosingComment { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary></summary>
                Public Class SingleComment
                End Class
                ''' <summary></summary>
                ''' <remarks></remarks>
                Public Class MultipleComment
                End Class
                ''' <inheritdoc />
                Public Class SelfClosingComment
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeatParameterized
                "should return comments when type has some" [
                (withProjects content, ("SingleComment", Set ["summary"]))
                (withProjects content, ("MultipleComment", Set ["summary"; "remarks"]))
                (withProjects content, ("SelfClosingComment", Set ["inheritdoc"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                    
                    test <@ result.Comments |> Seq.map (fun c -> c.Name)
                                            |> Set
                                            |> Set.isSubset expected @>)
        ]

    [<Tests>]
    let identicalCommentsTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                /// <typeparam></typeparam>
                public class SingleComment { }
                /// <typeparam></typeparam>
                /// <typeparam></typeparam>
                /// <typeparam></typeparam>
                public class MultipleComment { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <typeparam></typeparam>
                Public Class SingleComment
                End Class
                ''' <typeparam></typeparam>
                ''' <typeparam></typeparam>
                ''' <typeparam></typeparam>
                Public Class MultipleComment
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeatParameterized
                "should return all comments when type has identical comments type" [
                (withProjects content, ("SingleComment", 1))
                (withProjects content, ("MultipleComment", 3))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findType name
                    
                    test <@ result.Comments |> Seq.length = expected @>)
        ]