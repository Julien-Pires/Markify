namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_CommentsContent_Tests =
    [<Tests>]
    let commentContentTests =
        let content = [
            (ProjectLanguage.CSharp, ["
                /// <summary>foo</summary>
                /// <example><code>foo</code></example>
                /// <remarks>foo<code>foo</code></remarks>
                public class Comments { }
            "])
            (ProjectLanguage.VisualBasic, ["
                ''' <summary>foo</summary>
                ''' <example><code>foo</code></example>
                ''' <remarks>foo<code>foo</code></remarks>
                Public Class Comments
                End Class
            "])
        ]
        testList "Analyze/Comments" [
            yield! testRepeat (withProjects content)
                "should return text content when comment has some"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Comments"
                    let result = object.Comments.Comments |> Seq.find (fun c -> c.Name = "summary") 
                            
                    test <@ result.Content |> Seq.choose (function | Text c -> Some c | _ -> None) 
                                           |> Seq.head
                                           |> fun c -> c = "foo" @>)

            yield! testRepeat (withProjects content)
                "should return block content when comment has some"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Comments"
                    let result = object.Comments.Comments |> Seq.find (fun c -> c.Name = "example") 
                            
                    test <@ result.Content |> Seq.choose (function | Block c -> Some c | _ -> None) 
                                           |> Seq.head
                                           |> fun c -> c.Name = "code" @>)

            yield! testRepeat (withProjects content)
                "should return correct contents when comment has multiple type of contents"
                (fun sut project () ->
                    let assemblies = sut.Analyze project
                    let object = findClass assemblies "Comments"
                    let result = object.Comments.Comments |> Seq.find (fun c -> c.Name = "remarks") 
                            
                    test <@ result.Content |> Seq.length = 2 @>)
        ]