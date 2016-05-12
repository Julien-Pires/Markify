namespace Markify.Models

module Definitions =

    type StructureKind =  Class = 0 | Struct = 1 | Interface = 2 | Delegate = 3 | Enum = 4

    type DefinitionName = string
    type DefinitionFullname = string
    type DefinitionIdentity = {
        Name : DefinitionName
        Fullname : DefinitionFullname
    }

    type Modifier = string
    type ConstraintsList = string seq
    type GenericParameterDefinition = {
        Identity : DefinitionIdentity
        Modifier : Modifier
        Constraints : ConstraintsList
    }

    type ModifiersList = Modifier seq
    type BaseTypesList = string seq
    type GenericParametersList = GenericParameterDefinition seq
    type TypeDefinition = {
        Identity : DefinitionIdentity
        Kind : StructureKind
        AccessModifiers : ModifiersList
        Modifiers : ModifiersList
        BaseTypes : BaseTypesList
        Parameters : GenericParametersList
    }

    type TypesList = TypeDefinition seq
    type LibraryDefinition = {
        Types : TypesList
    }