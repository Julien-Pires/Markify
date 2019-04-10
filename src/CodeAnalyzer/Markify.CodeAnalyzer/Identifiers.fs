namespace Markify.CodeAnalyzer

type Name = string

type Modifier = string

type TypeName = string

type Value = string

type Constraint = string

type GenericInfo = {
    Name : Name
    Modifier : Modifier option
    Constraints : Constraint seq }

type ParameterInfo = {
    Name : Name
    Type : TypeName 
    Modifier : Modifier option
    DefaultValue : Value option }