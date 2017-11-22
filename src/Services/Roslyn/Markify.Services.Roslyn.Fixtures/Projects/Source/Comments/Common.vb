#Region "Comment Parameters"

''' <summary>
''' This Is a test summary
''' </summary>
Public Class WithNoParameter
End Class

''' <summary name="foo">
''' This Is a test summary
''' </summary>
Public Class WithOneParameter
End Class

''' <summary name="foo" value="bar" data>
''' This is a test summary
''' </summary>
Public Class WithMultipleParameter
End Class

#End Region

#Region "Class"

''' <summary></summary>
''' <remarks></remarks>
Public Class ClassWithEmptyComments(Of T, Y)
End Class

''' <summary>
''' Represents a foo type
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' </remarks>
''' <example>
''' This an example
''' </example>
''' <typeparam name="T">Parameter of type T</typeparam>
''' <typeparam name="Y">Parameter of type Y</typeparam>
''' <inheritdoc />
Public Class ClassWithSimpleComments(Of T, Y)
End Class

''' <summary>
''' Represents a foo type
''' <code>Some code here</code>
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' <code>Some code here for the type <see cref="ClassWithComplexComments"/></code>
''' </remarks>
Public Class ClassWithComplexComments(Of T, Y)
End Class

Public Class ClassWithoutComments(Of T, Y)
End Class

#End Region

#Region "Interface"

''' <summary></summary>
''' <remarks></remarks>
Public Interface InterfaceWithEmptyComments(Of T, Y)
End Interface

''' <summary>
''' Represents a foo type
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' </remarks>
''' <example>
''' This an example
''' </example>
''' <typeparam name="T">Parameter of type T</typeparam>
''' <typeparam name="Y">Parameter of type Y</typeparam>
''' <inheritdoc />
Public Interface InterfaceWithSimpleComments(Of T, Y)
End Interface

''' <summary>
''' Represents a foo type
''' <code>Some code here</code>
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' <code>Some code here for the type <see cref="InterfaceWithComplexComments"/></code>
''' </remarks>
Public Interface InterfaceWithComplexComments(Of T, Y)
End Interface

Public Interface InterfaceWithoutComments(Of T, Y)
End Interface

#End Region

#Region "Struct"

''' <summary></summary>
''' <remarks></remarks>
Public Structure StructWithEmptyComments(Of T, Y)
End Structure

''' <summary>
''' Represents a foo type
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' </remarks>
''' <example>
''' This an example
''' </example>
''' <typeparam name="T">Parameter of type T</typeparam>
''' <typeparam name="Y">Parameter of type Y</typeparam>
''' <inheritdoc />
Public Structure StructWithSimpleComments(Of T, Y)
End Structure

''' <summary>
''' Represents a foo type
''' <code>Some code here</code>
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' <code>Some code here for the type <see cref="StructWithComplexComments"/></code>
''' </remarks>
Public Structure StructWithComplexComments(Of T, Y)
End Structure

Public Structure StructWithoutComments(Of T, Y)
End Structure

#End Region

#Region "Enum"

''' <summary></summary>
''' <remarks></remarks>
Public Enum EnumWithEmptyComments
End Enum

''' <summary>
''' Represents a foo type
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' </remarks>
''' <example>
''' This an example
''' </example>
''' <inheritdoc />
Public Enum EnumWithSimpleComments
End Enum

''' <summary>
''' Represents a foo type
''' <code>Some code here</code>
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' <code>Some code here for the type <see cref="EnumWithComplexComments"/></code>
''' </remarks>
Public Enum EnumWithComplexComments
End Enum

Public Enum EnumWithoutComments
End Enum

#End Region

#Region "Delegate"

''' <summary></summary>
''' <remarks></remarks>
Public Delegate Sub DelegateWithEmptyComments(Of T, Y)()

''' <summary>
''' Represents a foo type
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' </remarks>
''' <example>
''' This an example
''' </example>
''' <typeparam name="T">Parameter of type T</typeparam>
''' <typeparam name="Y">Parameter of type Y</typeparam>
''' <inheritdoc />
Public Delegate Sub DelegateWithSimpleComments(Of T, Y)()

''' <summary>
''' Represents a foo type
''' <code>Some code here</code>
''' Contains only comments
''' </summary>
''' <remarks>
''' This type is used for testing comments
''' <code>Some code here for the type <see cref="DelegateWithComplexComments"/></code>
''' </remarks>
Public Delegate Sub DelegateWithComplexComments(Of T, Y)()

Public Delegate Sub DelegateWithoutComments(Of T, Y)()

#End Region