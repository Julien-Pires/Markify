namespace Markify.Services.Roslyn.Tests

open System.Text.RegularExpressions
open Markify.Domain.Compiler

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

    let private findModifier modifier (map : Map<'a, 'a>) = 
        match map.TryFind modifier with
        | Some x -> x
        | None -> modifier

    let normalizeSyntax modifier = findModifier modifier visualBasicModifiers

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

    let doesNameMatch name (definition : TypeDefinition) =
        let isFullName = Regex.Match(name, "\w+(\.\w+)+").Success
        let typeName =
            match isFullName with
            | true ->
                match (definition.Identity.Namespace, definition.Identity.Parents) with
                | (Some x, Some y) -> sprintf "%s.%s.%s" x y definition.Identity.Name
                | (Some x, _) | (_, Some x) -> sprintf "%s.%s" x definition.Identity.Name
                | _ -> definition.Identity.Name
            | false -> definition.Identity.Name
        typeName.EndsWith(name)

    let inline findByName name (items : ^a seq) =
        items |> Seq.filter(fun c -> (^a: (member Name : string) c) = name)

    let filterTypes assemblies name =
        assemblies.Types
        |> Seq.filter (doesNameMatch name)
        |> Seq.toList

    let findType assemblies name =
        assemblies.Types |> Seq.find (doesNameMatch name)

    let findClass assemblies name : ClassDefinition = 
        match findType assemblies name with
        | Class x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a class"))

    let findStruct assemblies name : ClassDefinition = 
        match findType assemblies name with
        | Struct x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a struct"))

    let findInterface assemblies name : ClassDefinition = 
        match findType assemblies name with
        | Interface x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an interface"))

    let findDelegate assemblies name : DelegateDefinition = 
        match findType assemblies name with
        | Delegate x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a delegate"))

    let findEnum assemblies name : EnumDefinition = 
        match findType assemblies name with
        | Enum x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an enum"))

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