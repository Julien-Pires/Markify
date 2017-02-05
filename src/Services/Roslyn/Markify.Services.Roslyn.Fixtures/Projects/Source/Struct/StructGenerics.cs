using System.Collections.Generic;

public struct SingleGenericType<T> { }

public struct MultipleGenericType<T, Y> 
    where T : class, IList<string>
    where Y : struct{ }