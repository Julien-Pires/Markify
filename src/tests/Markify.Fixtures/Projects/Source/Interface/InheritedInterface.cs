using System;
using System.Collections.Generic;

public interface IImplementIDisposable : IDisposable{}

public interface IImplementGenericInterface : IList<String>, IReadOnlyCollection<String>{}