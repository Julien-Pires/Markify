namespace Markify.CodeAnalyzer

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