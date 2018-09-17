namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler

module LanguageHelper =
    open Markify.Domain.Ide

    let cSharpModifiers =
        Map.empty.
            Add("public", "public").
            Add("private", "private").
            Add("protected", "protected").
            Add("internal", "internal").
            Add("partial", "partial").
            Add("sealed", "sealed").
            Add("abstract", "abstract").
            Add("static", "static").
            Add("virtual", "virtual").
            Add("override", "override").
            Add("in", "in").
            Add("out", "out").
            Add("class", "class").
            Add("struct", "struct").
            Add("const", "const").
            Add("readonly", "readonly").
            Add("ref", "ref").
            Add("new()", "new()")
    
    let visualBasicModifiers =
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

    let visualBasicMemberModifiers =
        [   visualBasicModifiers |> Map.toSeq; 
            Map.empty.
                Add("sealed", "NotOverridable").
                Add("static", "Shared").
                Add("out", "ByVal")
            |> Map.toSeq]
        |> Seq.concat
        |> Map

    let cSharpTypes =
        Map.empty.
            Add("void", "void")

    let visualBasicTypes =
        Map.empty.
            Add("void", "Void")

    let modifiers = 
        Map.empty.
            Add(ProjectLanguage.CSharp, cSharpModifiers).
            Add(ProjectLanguage.VisualBasic, visualBasicModifiers)

    let memberModifiers =
        Map.empty.
            Add(ProjectLanguage.CSharp, cSharpModifiers).
            Add(ProjectLanguage.VisualBasic, visualBasicMemberModifiers)
    
    let types =
        Map.empty.
            Add(ProjectLanguage.CSharp, cSharpTypes).
            Add(ProjectLanguage.VisualBasic, visualBasicTypes)

    let getModifier language m =
        let modifier = modifiers.[language]
        match modifier.TryFind m with
        | Some x -> x
        | None -> m

    let getMemberModifiers language modifier =
        memberModifiers.[language]
        |> Map.find modifier

    let getType language t =
        let types = types.[language]
        match types.TryFind t with
        | Some x -> x
        | None -> t

module DefinitionsHelper =
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

    let getFullname identity =
        match (identity.Namespace, identity.Parents) with
        | (Some x, Some y) -> sprintf "%s.%s.%s" x y identity.Name
        | (Some x, None) | (None, Some x) -> sprintf "%s.%s" x identity.Name
        | _ -> identity.Name

    let getBaseTypes = function
        | Class x | Struct x | Interface x -> x.Identity.BaseTypes
        | _ -> Seq.empty

    let getModifiers = function
        | Class x | Struct x | Interface x -> x.Identity.Modifiers
        | _ -> Seq.empty

    let getProperties = function
        | Class c | Struct c | Interface c -> c.Properties
        | _ -> Seq.empty

    let getFields = function
        | Class c | Struct c -> c.Fields
        | _ -> Seq.empty

    let getMethods = function
        | Class x | Struct x | Interface x -> x.Methods
        | _ -> Seq.empty

    let getEvents = function
        | Class x | Struct x | Interface x -> x.Events
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

    let getComments = function
        | Class x | Struct x | Interface x -> x.Comments.Comments
        | Enum x -> x.Comments.Comments
        | Delegate x -> x.Comments.Comments

    let getComment commentName definition =
        getComments definition
        |> Seq.tryFind (fun c -> c.Name = commentName)

    let getReturnType = function
        | Delegate x -> Some x.ReturnType
        | _ -> None

    let getParameters = function
        | Delegate x -> x.Parameters
        | _ -> Seq.empty