using System.Collections.Generic;

public class SingleGenericType<T> { }

public class MultipleGenericType<T, Y> 
    where T : class, IList<string>
    where Y : struct{ }