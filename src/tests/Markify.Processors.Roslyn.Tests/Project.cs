using System;
using System.Collections.Generic;

namespace Markify.Processors.Roslyn.Tests
{
    public sealed class Project
    {
        #region Properties

        public List<Uri> Code { get; set; }

        public int NamespaceCount { get; set; }

        public int ClassCount { get; set; }

        public int StructCount { get; set; }

        public int EnumCount { get; set; }

        public int DelegateCount { get; set; }

        #endregion
    }
}