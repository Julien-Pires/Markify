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

type TypeName = string
type Value = string
type AccessorDefinition = {
    AccessModifiers : Modifier seq }
type PropertyDefinition = {
    Name : DefinitionName
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option
    IsWrite : AccessorDefinition option
    IsRead : AccessorDefinition option }

type FieldDefinition = {
    Name : DefinitionName
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option }

type EnumValue = {
    Name : DefinitionName
    Value : Value option }

type EventDefinition = {
    Name : DefinitionName
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq }

type EnumDefinition = {
    Identity : TypeIdentity
    Values : EnumValue seq }

type ParameterDefinition = {
    Name : DefinitionName
    Type : TypeName 
    Modifier : Modifier option
    DefaultValue : Value option }

type DelegateDefinition = {
    Identity : TypeIdentity 
    Parameters : ParameterDefinition seq 
    ReturnType : TypeName }

type ClassDefinition = {
    Identity : TypeIdentity
    Fields : FieldDefinition seq
    Properties : PropertyDefinition seq
    Events : EventDefinition seq 
    Methods : DelegateDefinition seq }
type StructDefinition = ClassDefinition
type InterfaceDefinition = ClassDefinition

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