using System;

delegate void Do<in T>() where T : class, IDisposable, new();