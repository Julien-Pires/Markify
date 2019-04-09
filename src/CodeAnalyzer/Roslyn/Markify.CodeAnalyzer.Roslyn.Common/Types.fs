namespace Markify.CodeAnalyzer.Roslyn.Common

open Markify.CodeAnalyzer

[<Struct>]
type StructureMembers = {
    Fields : FieldInfo list
    Properties : PropertyInfo list
    Methods : DelegateDefinition list
    Events : EventInfo list }

[<Struct>]
type SourceContent = {
    Namespaces : NamespaceInfo list
    Types : TypeDefinition list }