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
type DefinitionIdentity = {
    Name : DefinitionName
    Parents : DefinitionName seq
    Namespace : DefinitionName option
}

type Modifier = string
type ConstraintsList = string seq
type GenericParameterDefinition = {
    Identity : DefinitionName
    Modifier : Modifier
    Constraints : ConstraintsList
}

type ModifiersList = Modifier seq
type BaseTypesList = string seq
type GenericParametersList = GenericParameterDefinition seq
type TypeDefinition = {
    Identity : DefinitionIdentity
    Kind : StructureKind
    AccessModifiers : ModifiersList
    Modifiers : ModifiersList
    BaseTypes : BaseTypesList
    Parameters : GenericParametersList
}

type NamespaceDefinition = {
    Name : DefinitionName
}

type LibraryDefinition = {
    Project : ProjectName
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq
}