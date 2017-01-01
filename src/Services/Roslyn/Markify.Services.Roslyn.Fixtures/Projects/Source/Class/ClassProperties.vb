Public MustInherit Class AbstractFooType
    Public Overridable Property SealedProperty() As Integer
End Class

Public Class FooType
    Inherits AbstractFooType

    Public Property AutoProperty()

    Public ReadOnly Property ReadOnlyProperty() As Integer

    Public WriteOnly Property WriteOnlyProperty() As Integer
        Set
        End Set
    End Property

    Public Property WithGetterModifierProperty() As Integer
        Friend Get
            Return 1
        End Get
        Set(value As Integer)
        End Set
    End Property

    Public Property WithSetterModifierProperty() As Integer
        Get
            Return 1
        End Get
        Friend Set(value As Integer)
        End Set
    End Property

    Public ReadOnly Property WithInitialValueProperty() As Integer = 1

    Protected Property WithModifierProperty() As Integer

    Property WithNoModifierProperty() As Integer

    Protected Friend Property WithMultipleModifiersProperty() As Integer

    Public Shared Property StaticProperty() As Integer

    Public Overridable Property VirtualProperty() As Integer

    Public NotOverridable Overrides Property SealedProperty() As Integer
End Class