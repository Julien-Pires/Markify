Imports System

Public Class FooType 
    Event PrivateEvent As EventHandler

    Friend Event InternalEvent As EventHandler

    Protected Friend Event ProtectedInternalEvent As EventHandler

    Public Shared Event StaticEvent As EventHandler

    Public Event GenericEvent As EventHandler(Of EventArgs)

    Public Custom Event ExplicitEvent As EventHandler
        AddHandler (ByVal value As EventHandler)
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
        End RemoveHandler
		RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
		End RaiseEvent
    End Event
End Class