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
    Parents : DefinitionName option
    Namespace : DefinitionName option }

type Modifier = string
type Constraint = string
type GenericParameterDefinition = {
    Identity : DefinitionName
    Modifier : Modifier
    Constraints : Constraint seq }

type BaseType = string
type TypeDefinition = {
    Identity : DefinitionIdentity
    Kind : StructureKind
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    BaseTypes : BaseType seq
    Parameters : GenericParameterDefinition seq }

type NamespaceDefinition = {
    Name : DefinitionName }

type LibraryDefinition = {
    Project : ProjectName
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq }