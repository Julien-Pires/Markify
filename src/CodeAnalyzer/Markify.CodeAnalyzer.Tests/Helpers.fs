namespace Markify.CodeAnalyzer.Tests

open System.Text.RegularExpressions
open Markify.CodeAnalyzer

[<AutoOpen>]
module LanguageHelper =
    let private visualBasicModifiers =
        Map.empty.
            Add("Public", "public").
            Add("Private", "private").
            Add("Protected", "protected").
            Add("Friend", "internal").
            Add("Partial", "partial").
            Add("NotInheritable", "sealed").
            Add("MustInherit", "abstract").
            Add("Static", "static").
            Add("Overridable", "virtual").
            Add("Overrides", "override").
            Add("In", "in").
            Add("Out", "out").
            Add("Class", "class").
            Add("Structure", "struct").
            Add("Dim", "Dim").
            Add("Const", "const").
            Add("ReadOnly", "readonly").
            Add("ByRef", "ref").
            Add("New", "new()").
            Add("NotOverridable", "sealed").
            Add("Shared", "static").
            Add("ByVal", "out").
            Add("Void", "void")

    let normalizeSyntax modifier = 
        match visualBasicModifiers.TryFind modifier with
        | Some x -> x
        | None -> modifier

[<AutoOpen>]
module TypeHelper =
    let objectTypes = [
        "Class"
        "Struct"
        "Interface"
        "Enum"
        "Delegate"] 

    //let getFullname definition =
    //    match (definition.Namespace, definition.Parents) with
    //    | (Some x, Some y) -> sprintf "%s.%s.%s" x y definition.Name
    //    | (Some x, None) | (None, Some x) -> sprintf "%s.%s" x definition.Name
    //    | _ -> definition.Name

    let doesNameMatch name (definition : Definition) =
        let typeName =
            match Regex.Match(name, "\w+(\.\w+)+").Success with
            | true -> definition.Name //getFullname definition
            | false -> definition.Name
        typeName = name

    let findType name assemblies =
        assemblies.Types |> Seq.find (doesNameMatch name)

    let findClass name assemblies =
        let definition = findType name assemblies
        match definition.Info with
        | Class x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a class"))

    let findStruct name assemblies = 
        let definition = findType name assemblies
        match definition.Info with
        | Struct x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a struct"))

    let findInterface name assemblies = 
        let definition = findType name assemblies
        match definition.Info with
        | Interface x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an interface"))

    let findDelegate name assemblies = 
        let definition = findType name assemblies
        match definition.Info with
        | Delegate x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a delegate"))

    let findEnum name assemblies = 
        let definition = findType name assemblies
        match definition.Info with
        | Enum x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an enum"))