namespace Markify.CodeAnalyzer

type NamespaceInfo = {
    Name : Name }

type AssemblyDefinition = {
    Project : Name
    Namespaces : NamespaceInfo seq
    Types : Definition seq }