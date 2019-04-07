namespace Markify.Services.Roslyn

open Markify.Domain.Compiler
open Markify.Services.Roslyn.Common

module MissingTypeMerger =
    let merge types (newTypes : TypeDefinition list) =
        List.fold (fun acc (c : TypeDefinition) ->
            let fullname = getFullname c.Identity
            match types |> Map.tryFind fullname with
            | Some _ -> acc
            | None -> acc |> Map.add fullname c) Map.empty newTypes

module PartialMerger =
    let isContainerType definition =
        match definition with
        | Class x | Struct x | Interface x -> Some (definition, x)
        | _ -> None

    let createTypeDefinition definition = function
        | Class _ -> Class definition
        | Struct _ -> Struct definition
        | _ -> Interface definition

    let isPartial partialKeyword (identity : TypeIdentity) =
        identity.Modifiers |> Seq.contains partialKeyword

    let mergeMethods (left : DelegateDefinition seq) (right : DelegateDefinition seq) isPartial =
        Seq.fold (fun acc (c : DelegateDefinition) ->
            match isPartial c.Identity with
            | true ->
                let methodDefinition = left |> Seq.tryFind (fun d -> d.Identity.Name = c.Identity.Name && d.Parameters = c.Parameters)
                match methodDefinition with
                | Some x -> acc
                | None -> c::acc
            | false -> c::acc) (left |> Seq.toList) right

    let mergeClass left right isPartial =
        let append x y = (x, y) ||> Seq.append |> Seq.toList
        let leftIdentity = left.Identity
        let rightIdentity = right.Identity
        let identity = 
            { left.Identity with
                Modifiers = Set <| Seq.append leftIdentity.Modifiers rightIdentity.Modifiers
                BaseTypes = Set <| Seq.append leftIdentity.BaseTypes rightIdentity.BaseTypes }
        {   left with
                Identity = identity
                Methods = mergeMethods left.Methods right.Methods isPartial
                Properties = append left.Properties right.Properties
                Fields = append left.Fields right.Fields
                Events = append left.Events right.Events }

    let merge types newTypes (languageSyntax : ILanguageSyntax) =
        let findDef fullname x = x |> Map.tryFind fullname |> Option.bind isContainerType
        newTypes
        |> List.choose isContainerType
        |> List.filter (fun c -> snd(c).Identity |> (isPartial languageSyntax.Partial))
        |> List.fold (fun acc c ->
            let typeDef, classDef = c
            let fullname = getFullname typeDef.Identity
            let definition =
                match findDef fullname acc with
                | None -> findDef fullname types
                | Some x -> Some x
            match definition with
            | Some x ->
                let mergedClass = mergeClass classDef (snd(x)) (isPartial languageSyntax.Partial)
                let mergedDefinition = createTypeDefinition mergedClass (fst(x))
                acc |> Map.add fullname mergedDefinition
            | _ -> acc |> Map.add fullname typeDef) Map.empty