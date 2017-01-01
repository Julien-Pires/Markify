Public Delegate Sub WithoutParameters()

Public Delegate Sub WithOneParameter(foo As Integer)

Public Delegate Sub WithMultipleParameters(foo As Integer, bar As Single)

Public Delegate Sub WithDefaultParameters(Optional foo As Integer = 1)

Public Delegate Sub WithParametersModifiers(ByRef foo As Integer, ByVal bar As Single)

Public Delegate Sub WithGenericParameters(Of T)(foo As T, bar As T())