namespace Markify.Models.Definitions

open Markify.Models.IDE

type StructureKind =  
    | Unknown = 0
    | Class = 1
    | Struct = 2 
    | Interface = 3 
    | Delegate = 4
    | Enum = 5

type DefinitionName = string

[<CustomEquality; NoComparison>]
type DefinitionIdentity = 
  { Name : DefinitionName
    Parents : DefinitionName option
    Namespace : DefinitionName option }

    override x.Equals(y) =
        match y with
        | :? DefinitionIdentity as w ->
            x.Name = w.Name && x.Parents = w.Parents && x.Namespace = w.Namespace
        | _ -> false

    override x.GetHashCode() =
        (31 * x.Name.GetHashCode()) * (31 * x.Parents.GetHashCode()) * (31 * x.Namespace.GetHashCode())

type Modifier = string
type ConstraintsList = string seq
type GenericParameterDefinition = {
    Identity : DefinitionName
    Modifier : Modifier
    Constraints : ConstraintsList }

type ModifiersList = Modifier seq
type BaseTypesList = string seq
type GenericParametersList = GenericParameterDefinition seq
type TypeDefinition = {
    Identity : DefinitionIdentity
    Kind : StructureKind
    AccessModifiers : ModifiersList
    Modifiers : ModifiersList
    BaseTypes : BaseTypesList
    Parameters : GenericParametersList }

type NamespaceDefinition = {
    Name : DefinitionName }

type LibraryDefinition = {
    Project : ProjectName
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq }