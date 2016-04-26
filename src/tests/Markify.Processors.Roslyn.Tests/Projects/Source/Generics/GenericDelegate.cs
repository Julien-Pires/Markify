using System;

delegate void Do<T>() where T : class, IDisposable, new();