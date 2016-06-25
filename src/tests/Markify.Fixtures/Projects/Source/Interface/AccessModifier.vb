Public Interface IPublicInterface
End Interface

Friend Interface IInternalInterface
End Interface

Public Interface IPrivateFoo
    Private Interface IPrivateInterface
    End Interface
End Interface

Public Interface IProtectedFoo
    Protected Interface IProtectedInterface
    End Interface
End Interface

Public Interface IProtectedInternalFoo
    Protected Friend Interface IProtectedInternalInterface
    End Interface
End Interface