namespace Markify.Services.Roslyn.Tests

open Markify.Domain.Compiler

module NamespaceHelper =
    let AllTypes = ["Class"; "Struct"; "Interface"; "Enum"; "Delegate"]
    let ContainerTypes = ["Class"; "Struct"; "Interface"]
    let ConcreteContainerTypes = ["Class"; "Struct"]

module TestHelper =
    let getDefinition name library = 
        library.Types |> Seq.find (fun c -> c.Identity.Name = name)

    let getDefinitions name library =
        library.Types |> Seq.filter (fun c -> c.Identity.Name = name)

    let getDefinitionByFullname fullname library =
        library.Types |> Seq.find (fun c -> (DefinitionsHelper.getFullname c.Identity) = fullname)

    let getDefinitionsByFullname fullname library =
        library.Types |> Seq.filter (fun c -> (DefinitionsHelper.getFullname c.Identity) = fullname)

    let filterDefinitions name namespaces library =
        namespaces
        |> Seq.map (fun c -> sprintf "%s.%s" c name)
        |> Seq.map (fun c -> getDefinitionByFullname c library)

    let getMethod name definitions =
        definitions
        |> Seq.map DefinitionsHelper.getMethods
        |> Seq.collect id
        |> Seq.filter (fun c -> c.Identity.Name = name)

    let getModifiers language modifiers =
        modifiers
        |> List.map (LanguageHelper.getModifier language)

    let getModifiersOld (modifiers : string) language =
        modifiers.Split (';') 
        |> Seq.map (LanguageHelper.getModifier language) 
        |> Set

    let getMemberModifiers (modifiers : string) language =
        modifiers.Split (';') 
        |> Seq.map (LanguageHelper.getMemberModifiers language) 
        |> Set

    let getGenericParameter name (definitions : TypeDefinition seq) =
        definitions
        |> Seq.map (fun c -> c.Identity.Parameters)
        |> Seq.collect id
        |> Seq.filter (fun c -> c.Name = name)

    let getProperty name definitions =
        definitions
        |> Seq.map DefinitionsHelper.getProperties
        |> Seq.collect id
        |> Seq.filter (fun c -> c.Name = name)

    let getField name definitions =
        definitions
        |> Seq.map DefinitionsHelper.getFields
        |> Seq.collect id
        |> Seq.filter (fun c -> c.Name = name)

    let getEvent name definitions =
        definitions
        |> Seq.map DefinitionsHelper.getEvents
        |> Seq.collect id
        |> Seq.filter (fun c -> c.Name = name)

    let getCommentParameter name parameter definitions =
        definitions
        |> DefinitionsHelper.getComment name
        |> fun c -> c.Value.Parameter
        |> Seq.find (fun c -> c.Name = parameter)

    let getDelegateParameter name definitions =
        definitions
        |> DefinitionsHelper.getParameters
        |> Seq.find (fun c -> c.Name = name)