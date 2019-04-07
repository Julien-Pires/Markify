namespace Markify.Services.Roslyn.VisualBasic

open System
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic.Syntax
open Microsoft.CodeAnalysis.VisualBasic
open Markify.Domain.Compiler

module CommentHelper =
    let getAttributeValue (attribute : XmlAttributeSyntax) =
        match attribute.Value with
        | :? XmlStringSyntax as x ->
            match x.TextTokens.Count with
            | 0 -> None
            | _ ->
                x.TextTokens
                |> Seq.map (fun c -> c.ValueText) 
                |> fun c -> String.Join("", c) |> Some
        | _ -> None
    
    let getNamedAttributeValue (attribute : XmlNameAttributeSyntax) =
        match attribute.Name with
        | null -> None
        | _ -> Some <| attribute.Name.ToString()

    let createParameterFromAttribute (attribute : XmlAttributeSyntax) =
        {   Name = attribute.Name.ToString()
            Value = getAttributeValue attribute }

    let createParameterFromNamedAttribute (attribute : XmlNameAttributeSyntax) =
        {   Name = attribute.Name.ToString()
            Value = getNamedAttributeValue attribute }

    let createParameter (attributes : SyntaxList<XmlNodeSyntax>) =
        attributes
        |> Seq.choose (fun c ->
            match c with
            | :? XmlAttributeSyntax as x -> Some <| createParameterFromAttribute x
            | :? XmlNameAttributeSyntax as x -> Some <| createParameterFromNamedAttribute x
            | _ -> None)
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
        findDocumentationTrivia node SyntaxKind.DocumentationCommentTrivia
        |> Seq.map (fun c -> createComment c)
        |> Seq.choose id
        |> Seq.toList