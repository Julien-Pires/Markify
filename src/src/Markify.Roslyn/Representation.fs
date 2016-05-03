module Representation
    open System
    open System.Linq

    type StructureKind =  Class = 0 | Struct = 1 | Interface = 2 | Delegate = 3 | Enum = 4

    type Fullname = list<string>
    type Modifier = string

    type ConstraintsList = list<string>
    type GenericParameterRepresentation = {
        Fullname: Fullname
        Modifier: Modifier option
        Constraints: ConstraintsList
    }

    type BaseTypesList = list<string>
    type ModifiersList = list<Modifier>
    type GenericParametersList = list<GenericParameterRepresentation>
    type TypeRepresentation = {
        Fullname: Fullname
        Kind: StructureKind
        AccessModifiers: ModifiersList
        AdditionalModifiers: ModifiersList
        GenericParameters: GenericParametersList
        BaseTypes: BaseTypesList
    }

    let Name (representation: TypeRepresentation) = representation.Fullname.Last()

    let toString (fullname: Fullname) = String.Join ("", fullname)