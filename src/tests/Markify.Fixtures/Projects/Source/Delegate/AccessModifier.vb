Public Delegate Sub PublicDelegate()

Friend Delegate Sub InternalDelegate()

Public Class PrivateFoo
    Private Delegate Sub PrivateDelegate()
End Class

Public Class ProtectedFoo
    Protected Delegate Sub ProtectedDelegate()
End Class

Public Class ProtectedInternalFoo
    Protected Friend Delegate Sub ProtectedInternalDelegate()
End Class