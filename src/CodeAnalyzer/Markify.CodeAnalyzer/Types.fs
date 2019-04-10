namespace Markify.CodeAnalyzer

type BaseType = string

type Identity = {
    Name : Name 
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    Generics : GenericInfo seq
    BaseType : BaseType }

type Hierarchy = {
    Namespace : Name option 
    Parent : Name option }

type DelegateInfo = {
    Parameters : ParameterInfo seq 
    ReturnType : TypeName }

type EnumInfo = {
    Values : EnumValue seq }

type StructureInfo = {
    Fields : FieldInfo seq
    Properties : PropertyInfo seq
    Events : EventInfo seq 
    Methods : MethodInfo seq }

type TypeInfo =
    | Class of StructureInfo
    | Struct of StructureInfo
    | Interface of StructureInfo
    | Delegate of DelegateInfo
    | Enum of EnumInfo

type Definition = {
    Identity : Identity
    Hierarchy : Hierarchy
    Info : TypeInfo
    Comments : Comments }