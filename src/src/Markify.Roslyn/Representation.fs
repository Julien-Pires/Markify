module Representation
    open System.Linq
    open System.Collections.Generic

    type StructureKind =  Class = 0 | Struct = 1 | Interface = 2 | Delegate = 3 | Enum = 4

    type Fullname = list<string>
    type Modifier = string

    type ConstraintsList = seq<string>
    type GenericParameterRepresentation = {
        Fullname: Fullname
        Modifier: Modifier
        Constraints: ConstraintsList
    }

    type BaseTypesList = seq<string>
    type ModifiersList = seq<Modifier>
    type GenericParametersList = seq<GenericParameterRepresentation>
    type TypeRepresentation = {
        Fullname: Fullname
        Kind: StructureKind
        AccessModifiers: ModifiersList
        AdditionalModifiers: ModifiersList
        GenericParameters: GenericParametersList
        BaseTypes: BaseTypesList
    }

    let Name (representation: TypeRepresentation) = representation.Fullname.Last()

