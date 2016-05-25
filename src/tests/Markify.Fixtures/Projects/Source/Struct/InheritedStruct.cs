using System;
using System.Collections.Generic;

public struct ImplementIDisposable : IDisposable{}

public struct ImplementGenericInterface : IList<String>, IReadOnlyCollection<String>{}