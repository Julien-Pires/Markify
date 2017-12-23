#Region "Class"

Namespace Class
    Public Class TypeWithFields
        Dim Field As Int32

        Dim FirstField, SecondField As Int32

        Shared StaticField As Int32

        Const ConstField As Int32 = 1

        ReadOnly ReadOnlyField As Int32

        Shared ReadOnly StaticReadOnlyField As Int32

        Public PublicField As Int32

        Private PrivateField As Int32

        Protected ProtectedField As Int32

        Friend InternalField As Int32

        Protected Friend ProtectedInternal As Int32
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure TypeWithNoField
    End Structure

    Public Structure TypeWithFields
        Dim Field As Int32

        Dim FirstField, SecondField As Int32

        Shared StaticField As Int32

        Const ConstField As Int32 = 1

        ReadOnly ReadOnlyField As Int32

        Shared ReadOnly StaticReadOnlyField As Int32

        Public PublicField As Int32

        Private PrivateField As Int32

        Friend InternalField As Int32
    End Structure
End Namespace

#End Region