Namespace CSharp.Project.Wellformed
    Public Delegate Sub TestDelegate()

    Public Delegate Function TestDelegate2() As Integer

    Public Delegate Sub TestDelegate3(foo As Integer)

    Public Delegate Function TestDelegate4(foo As Integer) As Integer

    Public Delegate Sub TestDelegate5(<[ParamArray]()> foo() As Integer)

    Public Delegate Function TestDelegate6(<[ParamArray]()> foo() As Integer) As Integer
End Namespace