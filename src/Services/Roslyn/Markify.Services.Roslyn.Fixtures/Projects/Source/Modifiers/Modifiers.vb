#Region "Class"

Namespace Class
    Public Class NoModifierType
    End Class

    Public MustInherit Class AbstractType
    End Class

    Partial Public Class PartialType
    End Class

    Public NotInheritable Class SealedType
    End Class

    Public Static Class StaticType
    End Class

    Partial Public MustInherit Class PartialAbstractType
    End Class

    Partial Public MustInherit Class AbstractPartialType
    End Class

    Partial Public NotInheritable Class PartialSealedType
    End Class

    Partial Public NotInheritable Class SealedPartialType
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure NoModifierType
    End Structure

    Partial Public Structure PartialType
    End Structure
End Namespace

#End Region

#Region "Inteface"

Namespace Interface
    Public Interface NoModifierType
    End Interface

    Partial Public Interface PartialType
    End Interface
End Namespace

#End Region