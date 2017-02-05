namespace Markify.Domain.Compiler

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

type TypeDefinition =
    | Class of ClassDefinition
    | Struct of ClassDefinition
    | Interface of ClassDefinition
    | Enum of EnumDefinition
    | Delegate of DelegateDefinition
    member this.Identity =
        match this with
        | Class x -> x.Identity
        | Struct x -> x.Identity
        | Interface x -> x.Identity
        | Enum x -> x.Identity
        | Delegate x -> x.Identity