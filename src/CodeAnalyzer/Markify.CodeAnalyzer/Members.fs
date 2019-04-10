namespace Markify.CodeAnalyzer

type AccessorInfo = {
    AccessModifiers : Modifier seq }

type PropertyInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option
    IsWrite : AccessorInfo option
    IsRead : AccessorInfo option }

type FieldInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option }

type MethodInfo = {
    Name : Name 
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    Generics : GenericInfo seq
    Parameters : ParameterInfo seq 
    ReturnType : TypeName }

type EventInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq }

type EnumValue = {
    Name : Name
    Value : Value option }