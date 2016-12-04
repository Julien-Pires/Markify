namespace Markify.Roslyn.Tests

module DefinitionsHelper =
    open Markify.Models.Definitions

    let (|IsClass|_|) definition =
        match definition with
        | Class x -> Some definition
        | _ -> None

    let (|IsStruct|_|) definition =
        match definition with
        | Struct x -> Some definition
        | _ -> None

    let (|IsInterface|_|) definition =
        match definition with
        | Interface x -> Some definition
        | _ -> None

    let (|IsEnum|_|) definition =
        match definition with
        | Enum x -> Some definition
        | _ -> None

    let (|IsDelegate|_|) definition =
        match definition with
        | Delegate x -> Some definition
        | _ -> None

    let getFilterByKind kind =
        match kind with
        | StructureKind.Class -> (|IsClass|_|)
        | StructureKind.Struct -> (|IsStruct|_|)
        | StructureKind.Interface -> (|IsInterface|_|)
        | StructureKind.Enum -> (|IsEnum|_|)
        | StructureKind.Delegate -> (|IsDelegate|_|)
        | _ -> fun c -> None

    let getFullname identity =
        match (identity.Namespace, identity.Parents) with
        | (Some x, Some y) -> sprintf "%s.%s.%s" x y identity.Name
        | (Some x, None) | (None, Some x) -> sprintf "%s.%s" x identity.Name
        | _ -> identity.Name

    let getProperties = function
        | Class c | Struct c | Interface c -> c.Properties
        | _ -> Seq.empty

    let getFields = function
        | Class c | Struct c -> c.Fields
        | _ -> Seq.empty

    let getProperty (definitions : TypeDefinition seq) name property =
        definitions
        |> Seq.find (fun d -> d.Identity.Name = name)
        |> getProperties
        |> Seq.find (fun d -> d.Name = property)

    let getField (definitions : TypeDefinition seq) name field =
        definitions
        |> Seq.find (fun d -> d.Identity.Name = name)
        |> getFields
        |> Seq.find (fun d -> d.Name = field)

    let getEnumValues = function
        | Enum c -> c.Values
        | _ -> Seq.empty

module TestHelper =
    let isSemanticEqual opt1 opt2 =
        match opt1, opt2 with
        | Some x, Some y -> true
        | None, None -> true
        | _ -> false
    