namespace Markify.Domain.Compiler

type NamespaceDefinition = {
    Name : DefinitionName }

type ProjectName = string
type AssemblyDefinition = {
    Project : ProjectName
    Namespaces : NamespaceDefinition seq
    Types : TypeDefinition seq }