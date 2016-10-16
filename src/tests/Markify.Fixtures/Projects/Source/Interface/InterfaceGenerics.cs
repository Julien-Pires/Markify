using System.Collections.Generic;

public interface SingleGenericType<T> { }

public interface MultipleGenericType<T, Y> 
    where T : class, IList<string>
    where Y : struct{ }

public interface CovariantGenericType<in T> { }

public interface ContravariantGenericType<out T> { }