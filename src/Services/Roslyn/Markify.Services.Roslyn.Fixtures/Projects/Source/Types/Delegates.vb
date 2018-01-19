Public Delegate Sub WithoutParameters()

Public Delegate Sub WithOneParameter(foo As Int32)

Public Delegate Sub WithMultipleParameters(foo As Int32, bar As Single)

Public Delegate Sub WithDefaultParameters(Optional foo As Int32 = 1)

Public Delegate Sub WithParametersModifiers(ByRef foo As Int32, ByVal bar As Single)

Public Delegate Sub WithGenericParameters(Of T)(foo As T, bar As T())

Public Delegate Function WithReturnType() As Int32

Public Delegate Function WithGenericReturnType(Of T)() As T

Public Delegate Sub SingleGenericType(Of T)()

Public Delegate Sub MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)()

Public Delegate Sub CovariantGenericType(Of In T)()

Public Delegate Sub ContravariantGenericType(Of Out T)()