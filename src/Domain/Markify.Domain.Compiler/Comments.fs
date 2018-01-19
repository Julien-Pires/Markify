namespace rec Markify.Domain.Compiler

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

type TypeComments = 
    { Comments : Comment seq }
    member private this.getComment name = 
        this.Comments 
        |> Seq.filter (fun c -> String.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase))
    member this.Summary = this.getComment "summary" |> Seq.tryHead
    member this.Remarks = this.getComment "remarks" |> Seq.tryHead
    member this.Example = this.getComment "example" |> Seq.tryHead
    member this.TypeParameters = this.getComment "typeparam"