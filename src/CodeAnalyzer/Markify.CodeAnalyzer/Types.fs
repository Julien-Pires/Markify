namespace Markify.CodeAnalyzer

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