module DefinitionsHelper
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