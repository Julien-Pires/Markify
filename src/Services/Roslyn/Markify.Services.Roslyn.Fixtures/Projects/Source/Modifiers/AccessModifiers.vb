#Region "Class"

Namespace Class
    Class NoAccessModifierType
    End Class

    Public Class PublicType
    End Class

    Friend Class InternalType
    End Class

    Partial Public Class ParentType
        Private Class PrivateType
        End Class

        Protected Class ProtectedType
        End Class

        Protected Friend Class ProtectedInternalType
        End Class

        Protected Friend Class InternalProtectedType
        End Class
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Structure NoAccessModifierType
    End Structure

    Public Structure PublicType
    End Structure

    Friend Structure InternalType
    End Structure

    Partial Public Structure ParentType
        Private Structure PrivateType
        End Structure

        Protected Structure ProtectedType
        End Structure

        Protected Friend Structure ProtectedInternalType
        End Structure

        Protected Friend Structure InternalProtectedType
        End Structure
    End Structure
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Interface NoAccessModifierType
    End Interface

    Public Interface PublicType
    End Interface

    Friend Interface InternalType
    End Interface

    Partial Public Interface ParentType
        Private Interface PrivateType
        End Interface

        Protected Interface ProtectedType
        End Interface

        Protected Friend Interface ProtectedInternalType
        End Interface

        Protected Friend Interface InternalProtectedType
        End Interface
    End Interface
End Namespace

#End Region

#Region "Enum"

Namespace Enum
    Enum NoAccessModifierType
    End Enum

    Public Enum PublicType
    End Enum

    Friend Enum InternalType
    End Enum

    Partial Public Class ParentType
        Private Enum PrivateType
        End Enum

        Protected Enum ProtectedType
        End Enum

        Protected Friend Enum ProtectedInternalType
        End Enum

        Protected Friend Enum InternalProtectedType
        End Enum
    End Class
End Namespace

#End Region

#Region "Delegate"

Namespace Delegate
    Delegate Sub NoAccessModifierType()

    Public Delegate Sub PublicType()

    Friend Delegate Sub InternalType()

    Partial Public Class ParentType
        Private Delegate Sub PrivateType()

        Protected Delegate Sub ProtectedType()

        Protected Friend Delegate Sub ProtectedInternalType()

        Protected Friend Delegate Sub InternalProtectedType()
    End Class
End Namespace

#End Region