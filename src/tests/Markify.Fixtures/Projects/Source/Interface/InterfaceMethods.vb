Public Interface FooType
    Sub Method()

    Function IntMethod() As Integer

    Sub WithParametersMethod(foo As Integer, ByRef bar As Integer, ByVal foobar As Integer)

    Function SingleGenericMethod(Of T)(Optional foo As T = Nothing) As T

    Function MultiGenericMethod(Of T As IList, Y As {IDisposable, IEnumerable})(foo As T, bar As y) As T
End Interface