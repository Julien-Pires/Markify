namespace Markify.Services.Roslyn.Common

open Markify.Domain.Compiler

type SourceContent = {
    Namespaces : NamespaceDefinition list
    Types : TypeDefinition list
}