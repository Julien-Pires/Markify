namespace Markify.Roslyn

open Markify.Models.Definitions

type SourceContent = {
    Namespaces : NamespaceDefinition list
    Types : TypeDefinition list
}