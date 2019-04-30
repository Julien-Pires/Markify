namespace rec Markify.CodeAnalyzer

type BaseType = string

type Hierarchy = {
    Name : Name 
    Parent : Hierarchy option }

type NamespaceInfo = {
    Name : Name
    Hierarchy : Hierarchy }

type Definition = {
    Name : Name 
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    Generics : GenericInfo seq
    BaseType : BaseType seq
    Hierarchy : Hierarchy
    Info : TypeInfo
    Comments : Comments }

type Assemblyinfo = {
    Project : Name
    Namespaces : NamespaceInfo seq
    Types : Definition seq }