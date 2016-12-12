namespace Markify.Roslyn

module PartialMerger =
    open Markify.Models.Definitions

    let mergeMethods (methodsOne : DelegateDefinition seq) (methodsTwo : DelegateDefinition seq) =
        methodsTwo
        |> Seq.fold (fun acc c ->
            let isPartial = 
                c.Identity.Modifiers
                |> Seq.tryFind (fun d -> d.ToLower() = "partial")
            match isPartial with
            | Some _ ->
                let methodDefinition =
                    methodsOne
                    |> Seq.tryFind (fun d -> d.Identity.Name = c.Identity.Name && d.Parameters = c.Parameters)
                match methodDefinition with
                | Some x -> acc
                | None -> c::acc
            | None -> c::acc) (methodsOne |> Seq.toList)

    let mergePartialDefinition definitionOne definitionTwo =
        let append x y =
            (x, y)
            ||> Seq.append
            |> Seq.toList
        let identity = 
            { definitionOne.Identity with
                Modifiers = Set <| Seq.append definitionOne.Identity.Modifiers definitionTwo.Identity.Modifiers
                BaseTypes = Set <| Seq.append definitionOne.Identity.BaseTypes definitionTwo.Identity.BaseTypes }
        {   definitionOne with
                Identity = identity
                Methods = mergeMethods definitionOne.Methods definitionTwo.Methods
                Properties = append definitionOne.Properties definitionTwo.Properties
                Fields = append definitionOne.Fields definitionTwo.Fields
                Events = append definitionOne.Events definitionTwo.Events }

    let getClassDefinition = function
        | Class x | Struct x | Interface x -> Some x
        | _ -> None

    let mergePartialTypes (types : TypeDefinition list) =
        let rec loop acc remainingTypes =
            match remainingTypes with
            | head::tail ->
                let newDefinition = mergePartialDefinition acc head
                loop newDefinition tail
            | _ -> acc
        let classDefinitions =
            types
            |> List.choose getClassDefinition
        match classDefinitions with
        | head::tail ->
            let classDefinition = loop head tail
            match types.Head with
            | Class _ -> Class classDefinition
            | Struct _ -> Struct classDefinition
            | _ -> Interface classDefinition
        | _ -> types.Head