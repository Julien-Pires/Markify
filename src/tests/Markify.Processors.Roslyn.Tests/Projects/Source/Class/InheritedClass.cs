using System;
using System.Collections.Generic;

public class InheritClass : Exception{}

public class ImplementInterfaceClass : IDisposable{}

public class ImplementGenInterfaceClass : IList<String>, IReadOnlyCollection<String>{}

public class MixedInheritanceClass : Exception, IList<String>, IDisposable{}