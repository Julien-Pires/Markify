Public Class FooType
    Sub Method()
	End Sub

    Public Sub PublicMethod()
	End Sub

    Friend Protected Sub InternalProtectedMethod()
	End Sub

    Function IntMethod() As Integer
		Return 1
	End Function

    Public Overridable Sub VirtualMethod()
	End Sub

    Private Partial Sub PartialMethod()
    End Sub

    Sub WithParametersMethod(foo As Integer, ByRef bar As Integer, ByVal foobar As Integer)
	End Sub

    Function SingleGenericMethod(Of T)(Optional foo As T = Nothing) As T
		Return CType(Nothing, T) 
	End Function

    Function MultiGenericMethod(Of T As IList, Y As {IDisposable, IEnumerable})(foo As T, bar As y) As T
		Return CType(Nothing, T) 
	End Function
End Class