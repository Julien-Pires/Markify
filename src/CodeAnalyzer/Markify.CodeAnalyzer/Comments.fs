namespace rec Markify.CodeAnalyzer

open System

type CommentName = string
type CommentText = string
type CommentParameterName = string
type CommentParameterValue = string

type CommentParameter = {
    Name : CommentParameterName
    Value : CommentParameterValue option }

type CommentContent = 
    | Text of string
    | Block of Comment

type Comment = {
    Name : CommentName
    Content : CommentContent seq
    Parameter : CommentParameter seq }

type Comments = Comment seq