Imports System
Imports System.Collections

Public Partial Interface FooType
    Inherits IDisposable
    Property PropertyOne As Integer

    Event Done As EventHandler

    Sub MethodOne(foo As Integer)
End Interface

Public Partial Interface FooType
    Inherits IEnumerable
    Property PropertyTwo As Integer

    Event Started As EventHandler

    Sub MethodTwo(foo As Integer, bar As Integer)
End Interface