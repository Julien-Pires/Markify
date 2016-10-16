Public Interface PublicType
End Interface

Friend Interface InternalType
End Interface

Public Partial Interface ParentType
    Private Interface PrivateType
    End Interface

    Protected Interface ProtectedType
    End Interface

    Protected Friend Interface ProtectedInternalType
    End Interface

    Friend Protected Interface InternalProtectedType
    End Interface
End Interface