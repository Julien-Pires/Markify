using System;
using System.Collections.Generic;

public class InheritType : Exception { }

public class ImplementInterfaceType : IDisposable { }

public class ImplementGenericInterfaceType : IList<String> { }

public class MixedInheritanceType : Exception, IDisposable { }