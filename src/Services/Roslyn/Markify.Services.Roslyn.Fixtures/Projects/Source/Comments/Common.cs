#region Comment Parameters

/// <summary>
/// This is a test summary
/// </summary>
public class WithNoParameter { }

/// <summary name="foo">
/// This is a test summary
/// </summary>
public class WithOneParameter { }

/// <summary name="foo" value="bar" data>
/// This is a test summary
/// </summary>
public class WithMultipleParameter { }

#endregion

#region Class

/// <summary></summary>
/// <remarks></remarks>
public class ClassWithEmptyComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// </remarks>
/// <example>
/// This an example
/// </example>
/// <typeparam name="T">Parameter of type T</typeparam>
/// <typeparam name="Y">Parameter of type Y</typeparam>
/// <inheritdoc />
public class ClassWithSimpleComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// <code>Some code here</code>
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// <code>Some code here for the type <see cref="ClassWithComplexComments"/></code>
/// </remarks>
public class ClassWithComplexComments<T, Y>
{
}

public class ClassWithoutComments<T, Y>
{
}

#endregion

#region Interface

/// <summary></summary>
/// <remarks></remarks>
public interface InterfaceWithEmptyComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// </remarks>
/// <example>
/// This an example
/// </example>
/// <typeparam name="T">Parameter of type T</typeparam>
/// <typeparam name="Y">Parameter of type Y</typeparam>
/// <inheritdoc />
public interface InterfaceWithSimpleComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// <code>Some code here</code>
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// <code>Some code here for the type <see cref="InterfaceWithComplexComments"/></code>
/// </remarks>
public interface InterfaceWithComplexComments<T, Y>
{
}

public interface InterfaceWithoutComments<T, Y>
{
}

#endregion

#region Struct

/// <summary></summary>
/// <remarks></remarks>
public struct StructWithEmptyComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// </remarks>
/// <example>
/// This an example
/// </example>
/// <typeparam name="T">Parameter of type T</typeparam>
/// <typeparam name="Y">Parameter of type Y</typeparam>
/// <inheritdoc />
public struct StructWithSimpleComments<T, Y>
{
}

/// <summary>
/// Represents a foo type
/// <code>Some code here</code>
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// <code>Some code here for the type <see cref="StructWithComplexComments"/></code>
/// </remarks>
public struct StructWithComplexComments<T, Y>
{
}

public struct StructWithoutComments<T, Y>
{
}

#endregion

#region Enum

/// <summary></summary>
/// <remarks></remarks>
public enum EnumWithEmptyComments
{
}

/// <summary>
/// Represents a foo type
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// </remarks>
/// <example>
/// This an example
/// </example>
/// <inheritdoc />
public enum EnumWithSimpleComments
{
}

/// <summary>
/// Represents a foo type
/// <code>Some code here</code>
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// <code>Some code here for the type <see cref="EnumWithComplexComments"/></code>
/// </remarks>
public enum EnumWithComplexComments
{
}

public enum EnumWithoutComments
{
}

#endregion

#region Delegate

/// <summary></summary>
/// <remarks></remarks>
public delegate void DelegateWithEmptyComments<T, Y>();

/// <summary>
/// Represents a foo type
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// </remarks>
/// <example>
/// This an example
/// </example>
/// <typeparam name="T">Parameter of type T</typeparam>
/// <typeparam name="Y">Parameter of type Y</typeparam>
/// <inheritdoc />
public delegate void DelegateWithSimpleComments<T, Y>();

/// <summary>
/// Represents a foo type
/// <code>Some code here</code>
/// Contains only comments
/// </summary>
/// <remarks>
/// This type is used for testing comments
/// <code>Some code here for the type <see cref="DelegateWithComplexComments"/></code>
/// </remarks>
public delegate void DelegateWithComplexComments<T, Y>();

public delegate void DelegateWithoutComments<T, Y>();

#endregion