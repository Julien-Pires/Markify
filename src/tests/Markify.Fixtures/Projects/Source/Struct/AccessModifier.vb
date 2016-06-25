Public Structure PublicStruct
End Structure

Friend Structure InternalStruct
End Structure

Public Structure PrivateFoo
    Private Structure PrivateStruct
    End Structure
End Structure

Public Class ProtectedFoo
    Protected Structure ProtectedStruct
    End Structure
End Class

Public Class ProtectedInternalFoo
    Protected Friend Structure ProtectedInternalStruct
    End Structure
End Class