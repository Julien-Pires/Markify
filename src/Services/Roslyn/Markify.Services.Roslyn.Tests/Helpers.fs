namespace Markify.Services.Roslyn.Tests

open System.Text.RegularExpressions
open Markify.Domain.Compiler
open Markify.Domain.Ide

[<AutoOpen>]
module LanguageHelper =
    let private visualBasicModifiers =
        Map.empty.
            Add("public", "Public").
            Add("private", "Private").
            Add("protected", "Protected").
            Add("internal", "Friend").
            Add("partial", "Partial").
            Add("sealed", "NotInheritable").
            Add("abstract", "MustInherit").
            Add("static", "Static").
            Add("virtual", "Overridable").
            Add("override", "Overrides").
            Add("in", "In").
            Add("out", "Out").
            Add("class", "Class").
            Add("struct", "Structure").
            Add("Dim", "Dim").
            Add("const", "Const").
            Add("readonly", "ReadOnly").
            Add("ref", "ByRef").
            Add("new()", "New")
    
    let private visualBasicMemberModifiers =
        Map.empty.
            Add("sealed", "NotOverridable").
            Add("static", "Shared").
            Add("out", "ByVal")

    let private visualBasicTypes =
        Map.empty.
            Add("void", "Void")

    let private findModifier modifier (map : Map<'a, 'a>) = function
        | ProjectLanguage.CSharp -> modifier
        | _ ->
            match map.TryFind modifier with
            | Some x -> x
            | None -> modifier

    let getModifier modifier = findModifier modifier visualBasicModifiers

    let getMemberModifiers language modifier = findModifier modifier visualBasicMemberModifiers

    let getType typename = findModifier typename visualBasicTypes

[<AutoOpen>]
module TypeHelper =
    let objectTypes = [
        "Class"
        "Struct"
        "Interface"
        "Enum"
        "Delegate"]
        
    let inline findByName name (items : ^a seq) =
        items |> Seq.filter(fun c -> (^a: (member Name : string) c) = name)

    let filterTypes assemblies name =
        let isFullName = Regex.Match(name, "\w+(\.\w+)+").Success
        assemblies.Types
        |> Seq.filter (fun c ->
            let typeName =
                match isFullName with
                | true ->
                    match (c.Identity.Namespace, c.Identity.Parents) with
                    | (Some x, Some y) -> sprintf "%s.%s.%s" x y c.Identity.Name
                    | (Some x, _) | (_, Some x) -> sprintf "%s.%s" x c.Identity.Name
                    | _ -> c.Identity.Name
                | false -> c.Identity.Name
            typeName.EndsWith(name))
        |> Seq.toList

    let getProperties = function
        | Class c | Struct c | Interface c -> c.Properties
        | _ -> Seq.empty

    let getProperty name definitions =
        definitions
        |> Seq.map getProperties
        |> Seq.collect id
        |> findByName name

    let getGenericParameter name (definitions : TypeDefinition seq) =
        definitions
        |> Seq.map (fun c -> c.Identity.Parameters)
        |> Seq.collect id
        |> findByName name