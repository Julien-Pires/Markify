Namespace CSharp.Project.Wellformed
    Public Class TestClass
        Protected Class InnerTestClass
        End Class 
    End Class 

    Public NotInheritable Class TestClass1
    End Class 

    Friend MustInherit Class TestClass2
    End Class 

    Public NotInheritable Partial class TestClass3
    End Class 

    Public NotInheritable Partial class TestClass3
    End Class 

    Public MustInherit Partial class TestClass5
    End Class 

    Public MustInherit Partial class TestClass5
    End Class 

    Public Module TestClass7
    End Module
End Namespace