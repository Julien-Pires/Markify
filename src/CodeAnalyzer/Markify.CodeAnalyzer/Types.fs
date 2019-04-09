namespace rec Markify.CodeAnalyzer

type Constraint = string
type GenericDefinition = {
    Name : Name
    Modifier : Modifier option
    Constraints : Constraint seq }

type BaseType = string
type Identity = {
    Name : Name 
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq 
    Generics : GenericDefinition seq
    BaseType : BaseType seq }

type Hierarchy = {
    Namespace : Name option 
    Parent : Name option }

type ParameterDefinition = {
    Name : Name
    Type : TypeName 
    Modifier : Modifier option
    DefaultValue : Value option }

type DelegateInfo = {
    Parameters : ParameterDefinition seq 
    ReturnType : TypeName 
    Comments : Comments }

type EnumInfo = {
    Values : EnumValue seq
    Comments : Comments }

type StructureInfo = {
    Fields : FieldInfo seq
    Properties : PropertyInfo seq
    Events : EventInfo seq 
    Methods : Definition seq
    Comments : Comments }

type TypeInfo =
    | Class of StructureInfo
    | Struct of StructureInfo
    | Interface of StructureInfo
    | Delegate of DelegateInfo
    | Enum of EnumInfo

type Definition = {
    Identity : Identity
    Hierarchy : Hierarchy
    Info : TypeInfo }