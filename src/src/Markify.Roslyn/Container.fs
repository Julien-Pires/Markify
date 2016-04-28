module Container
    open Representation

    type StructureContainer = {
        Representation: TypeRepresentation
    }

    let Fullname (container: StructureContainer) = container.Representation.Fullname
