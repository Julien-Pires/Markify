namespace rec Markify.CodeAnalyzer

type BaseType = string

type Identity = {
    Name : Name 
    AccessModifiers : Modifier seq
    Modifiers : Modifier seq
    Generics : GenericInfo seq
    BaseType : BaseType }

type Hierarchy = {
    Name : Name 
    Parent : Hierarchy option }

type NamespaceInfo = {
    Name : Name
    Hierarchy : Hierarchy }

type Definition = {
    Identity : Identity
    Hierarchy : Hierarchy
    Info : TypeInfo
    Comments : Comments }

type Assemblyinfo = {
    Project : Name
    Namespaces : NamespaceInfo seq
    Types : Definition seq }