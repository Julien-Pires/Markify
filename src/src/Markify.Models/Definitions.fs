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
type Modifier = string
type Constraint = string
type GenericParameterDefinition = {
    Name : DefinitionName
    Modifier : Modifier option
    Constraints : Constraint seq }

type BaseType = string
type TypeIdentity = {
    Name : DefinitionName
    Parents : DefinitionName option
    Namespace : DefinitionName option
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    BaseTypes : BaseType seq
    Parameters : GenericParameterDefinition seq }

type ClassDefinition = {
    Identity : TypeIdentity }
type StructDefinition = ClassDefinition
type InterfaceDefinition = ClassDefinition
type EnumDefinition = ClassDefinition
type DelegateDefinition = ClassDefinition

type TypeDefinition =
    | Class of ClassDefinition
    | Struct of StructDefinition
    | Interface of InterfaceDefinition
    | Enum of EnumDefinition
    | Delegate of DelegateDefinition
    member this.Identity =
        match this with
        | Class x -> x.Identity
        | Struct x -> x.Identity
        | Interface x -> x.Identity
        | Enum x -> x.Identity
        | Delegate x -> x.Identity

type NamespaceDefinition = {
    Name : DefinitionName }

type LibraryDefinition = {
    Project : ProjectName
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq }