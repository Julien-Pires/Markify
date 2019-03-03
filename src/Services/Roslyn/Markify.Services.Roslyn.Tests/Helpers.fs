namespace Markify.Services.Roslyn.Tests

open System.Text.RegularExpressions
open Markify.Domain.Compiler

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

    let getFullname identity =
        match (identity.Namespace, identity.Parents) with
        | (Some x, Some y) -> sprintf "%s.%s.%s" x y identity.Name
        | (Some x, None) | (None, Some x) -> sprintf "%s.%s" x identity.Name
        | _ -> identity.Name

    let doesNameMatch name (definition : TypeDefinition) =
        let typeName =
            match Regex.Match(name, "\w+(\.\w+)+").Success with
            | true -> getFullname definition.Identity
            | false -> definition.Identity.Name
        typeName = name

    let findType assemblies name =
        assemblies.Types |> Seq.find (doesNameMatch name)

    let findClass name assemblies : ClassDefinition = 
        match findType assemblies name with
        | Class x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a class"))

    let findStruct name assemblies : ClassDefinition = 
        match findType assemblies name with
        | Struct x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a struct"))

    let findInterface name assemblies : ClassDefinition = 
        match findType assemblies name with
        | Interface x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an interface"))

    let findDelegate name assemblies : DelegateDefinition = 
        match findType assemblies name with
        | Delegate x -> x
        | _ -> raise (System.InvalidCastException("Found type is not a delegate"))

    let findEnum name assemblies : EnumDefinition = 
        match findType assemblies name with
        | Enum x -> x
        | _ -> raise (System.InvalidCastException("Found type is not an enum"))