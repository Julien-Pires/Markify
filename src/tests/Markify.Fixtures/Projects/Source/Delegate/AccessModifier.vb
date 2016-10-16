Public Delegate Sub PublicType()

Friend Delegate Sub InternalType()

Public Partial Class ParentType
    Private Delegate Sub PrivateType()

    Protected Delegate Sub ProtectedType()

    Protected Friend Delegate Sub ProtectedInternalType()

    Friend Protected Delegate Sub InternalProtectedType()
End Class