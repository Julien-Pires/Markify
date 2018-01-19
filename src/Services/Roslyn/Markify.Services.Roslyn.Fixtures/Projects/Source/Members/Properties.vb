#Region "Class"

Namespace Class
    Public Class TypeWithNoProperties
    End Class

    Public MustInherit Class AbstractTypeWithProperties
        Public Overridable Property SealedProperty() As Int32
    End Class

    Public Class TypeWithProperties
        Inherits AbstractFooType

        Public Property AutoProperty() As Int32

        Public ReadOnly Property ReadOnlyProperty() As Int32

        Public WriteOnly Property WriteOnlyProperty() As Int32
            Set
            End Set
        End Property

        Public Property WithGetterModifierProperty() As Int32
            Friend Get
                Return 1
            End Get
            Set(value As Int32)
            End Set
        End Property

        Public Property WithSetterModifierProperty() As Int32
            Get
                Return 1
            End Get
            Friend Set(value As Int32)
            End Set
        End Property

        Public ReadOnly Property WithInitialValueProperty() As Int32 = 1

        Private Property PrivateProperty() As Int32

        Protected Property ProtectedProperty() As Int32

        Friend Property InternalProperty() As Int32

        Protected Friend Property ProtectedInternalProperty() As Int32

        Property WithNoModifierProperty() As Int32

        Public Shared Property StaticProperty() As Int32

        Public Overridable Property VirtualProperty() As Int32

        Public NotOverridable Overrides Property SealedProperty() As Int32
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure TypeWithNoProperties
    End Structure

    Public Structure TypeWithProperties
        Public Property AutoProperty() As Int32

        Public ReadOnly Property ReadOnlyProperty() As Int32

        Public WriteOnly Property WriteOnlyProperty() As Int32
            Set
            End Set
        End Property

        Public Property WithGetterModifierProperty() As Int32
            Friend Get
                Return 1
            End Get
            Set(value As Int32)
            End Set
        End Property

        Public Property WithSetterModifierProperty() As Int32
            Get
                Return 1
            End Get
            Friend Set(value As Int32)
            End Set
        End Property

        Public Shared ReadOnly Property WithInitialValueProperty() As Int32 = 1

        Private Property PrivateProperty() As Int32

        Friend Property InternalProperty() As Int32

        Property WithNoModifierProperty() As Int32

        Public Shared Property StaticProperty() As Int32
    End Structure
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface TypeWithNoProperties
    End Interface

    Public Interface TypeWithProperties
        Property AutoProperty() As Int32

        ReadOnly Property ReadOnlyProperty() As Int32

        WriteOnly Property WriteOnlyProperty() As Int32

        Property WithNoModifierProperty()
    End Interface
End Namespace

#End Region