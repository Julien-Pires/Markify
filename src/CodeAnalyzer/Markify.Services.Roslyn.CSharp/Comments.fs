namespace Markify.Services.Roslyn.Csharp

open System
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.CSharp
open Markify.Domain.Compiler

module CommentHelper =
    let getParameterValue (attribute : XmlAttributeSyntax) =
        let value = 
            match attribute with
            | :? XmlNameAttributeSyntax as x -> x.Identifier.ToString()
            | :? XmlTextAttributeSyntax as x -> x.TextTokens |> Seq.fold (fun acc c -> acc + c.ToString()) ""
            | _ -> ""
        match value with
        | "" -> None
        | _ -> Some value

    let createParameter (attributes : SyntaxList<XmlAttributeSyntax>) =
        attributes
        |> Seq.map (fun c ->
            {   Name = c.Name.ToString()
                Value = getParameterValue c })
        |> Seq.toList

    let createCommentFromEmptyTag (element : XmlEmptyElementSyntax) =
        {   Comment.Name = element.Name.ToString()
            Content = []
            Parameter = createParameter element.Attributes }
       
    let rec createCommentFromTag (element : XmlElementSyntax) =
        let contents =
            element.Content
            |> Seq.choose (fun c ->
                match c with
                | :? XmlTextSyntax as x -> Some <| Text (String.Join ("", x.ChildTokens()))
                | :? XmlElementSyntax as x -> Some <| Block (createCommentFromTag x)
                | :? XmlEmptyElementSyntax as x -> Some <| Block (createCommentFromEmptyTag x)
                | _ -> None)
            |> Seq.toList
        {   Comment.Name = element.StartTag.Name.ToString()
            Content = contents
            Parameter = createParameter element.StartTag.Attributes }

    let createComment (element : XmlNodeSyntax) =
        match element with
        | :? XmlElementSyntax as x -> Some <| createCommentFromTag x
        | :? XmlEmptyElementSyntax as x -> Some <| createCommentFromEmptyTag x
        | _ -> None

open CommentHelper

module CommentBuilder =
    let findDocumentationTrivia (node : SyntaxNode) kind = 
        node.GetLeadingTrivia()
        |> Seq.filter (fun c -> c.Kind() = kind)
        |> Seq.collect(fun c -> (c.GetStructure() :?> DocumentationCommentTriviaSyntax).ChildNodes())
        |> Seq.choose (fun c ->
            match c with
            | :? XmlElementSyntax | :? XmlEmptyElementSyntax -> Some (c :?> XmlNodeSyntax)
            | _ -> None)
    
    let getComments node =
        findDocumentationTrivia node SyntaxKind.SingleLineDocumentationCommentTrivia
        |> Seq.map (fun c -> createComment c)
        |> Seq.choose id
        |> Seq.toList