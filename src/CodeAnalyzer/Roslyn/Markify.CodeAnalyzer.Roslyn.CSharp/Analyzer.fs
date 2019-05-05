namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn
open Microsoft.CodeAnalysis.CSharp

[<Language("CSharp")>]
type CSharpAnalyzer() =
    interface ISourceAnalyzer with
        member __.Analyze source =
            let tree = CSharpSyntaxTree.ParseText source.Content
            let root = tree.GetRoot()
            let types =
                TypesCollector().Visit(root)
                |> Seq.map TypeFactory.create
                |> Seq.choose (function | Success x -> Some x | _ -> None)
                |> Seq.map (fun c ->
                    let info =
                        match c.Members with
                        | Structure x -> 
                            let info = {
                                StructureInfo.Fields = x.Fields |> List.map (fun c -> c.Value)
                                Properties = x.Properties |> List.map (fun c -> c.Value)
                                Events = x.Events |> List.map (fun c -> c.Value)
                                Methods = x.Methods |> List.map (fun c -> c.Value) }
                            match c.Source with
                            | IsClass _ -> Class info
                            | IsStruct _ -> Struct info
                            | _ -> Interface info
                        | Enum x -> TypeInfo.Enum { EnumInfo.Values = x.Values |> List.map (fun c -> c.Value) }
                        | Delegate x -> TypeInfo.Delegate {
                            DelegateInfo.Parameters = x.Parameters |> List.map (fun c -> c.Value)
                            ReturnType = x.ReturnType }
                    {   Name = c.Identity.Name
                        AccessModifiers = c.Identity.AccessModifiers
                        Modifiers = c.Identity.Modifiers 
                        Generics = c.Identity.Generics 
                        BaseType = c.Identity.BaseType
                        Info = info 
                        Hierarchy = { Name = c.Identity.Name; Parent = None }
                        Comments = [] })
                |> Seq.map (fun c -> { Definition = c; IsPartial = false })
                |> Seq.toList
            {   Namespaces = []
                Definitions = types }