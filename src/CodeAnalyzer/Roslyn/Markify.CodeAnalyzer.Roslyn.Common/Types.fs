namespace Markify.CodeAnalyzer.Roslyn.Common

open Markify.CodeAnalyzer

[<Struct>]
type StructureMembers = {
    Fields : FieldDefinition list
    Properties : PropertyDefinition list
    Methods : DelegateDefinition list
    Events : EventDefinition list }

[<Struct>]
type SourceContent = {
    Namespaces : NamespaceDefinition list
    Types : TypeDefinition list }