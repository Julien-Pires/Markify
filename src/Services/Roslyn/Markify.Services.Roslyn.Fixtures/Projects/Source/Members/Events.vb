#Region "Class"

Namespace Class
    Public Class TypeWithNoEvent
    End Class

    Public MustInherit Class AbstractTypeWithEvents
        Public MustInherit Event AbstractEvent As EventHandler
    End Class

    Public Class TypeWithEvents
        Inherits AbstractTypeWithEvents

        Event Event As EventHandler

        Private PrivateEvent As EventHandler

        Friend Event InternalEvent As EventHandler

        Protected Friend Event ProtectedInternalEvent As EventHandler

        Protected Event ProtectedEvent As EventHandler

        Public Shared Event StaticEvent As EventHandler

        Public Overridable Event VirtualEvent As EventHandler

        Public NotOverridable Overrides Event SealedEvent As EventHandler

        Public Event GenericEvent As EventHandler(Of EventArgs)

        Public Custom Event ExplicitEvent As EventHandler
            AddHandler(ByVal value As EventHandler)
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            End RaiseEvent
        End Event
    End Class
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface TypeWithNoEvent
    End Interface

    Public Interface TypeWithEvents
        Event Event As EventHandler

        Event GenericEvent As EventHandler(Of EventArgs)
    End Interface
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure TypeWithNoEvent
    End Structure

    Public Structure TypeWithEvents
        Event Event As EventHandler

        Private Event PrivateEvent As EventHandler

        Friend Event InternalEvent As EventHandler

        Public Shared Event StaticEvent As EventHandler

        Public Event GenericEvent As EventHandler(Of EventArgs)

        Public Custom Event ExplicitEvent As EventHandler
            AddHandler(ByVal value As EventHandler)
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            End RaiseEvent
        End Event
    End Structure
End Namespace

#End Region