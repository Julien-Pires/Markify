module Container
    open Representation

    type TypeContainer = {
        Representation: TypeRepresentation
    }
    type TypeContainerList = TypeContainer seq

    let Fullname (container: TypeContainer) = container.Representation.Fullname
