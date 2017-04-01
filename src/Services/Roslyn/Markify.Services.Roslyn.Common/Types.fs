namespace Markify.Services.Roslyn.Common

open Markify.Domain.Compiler

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