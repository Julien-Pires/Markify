namespace Markify.CodeAnalyzer

type Value = string

type AccessorDefinition = {
    AccessModifiers : Modifier seq }
type PropertyInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option
    IsWrite : AccessorDefinition option
    IsRead : AccessorDefinition option }

type FieldInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    DefaultValue : Value option }

type EventInfo = {
    Name : Name
    Type : TypeName
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq }

type EnumValue = {
    Name : Name
    Value : Value option }