Imports System

Public Interface FooType
    Event PrivateEvent As EventHandler

    Event GenericEvent As EventHandler(Of EventArgs)
End Interface