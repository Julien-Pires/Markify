using System;
using System.Collections.Generic;

namespace Markify.Models
{
    public sealed class ProjectContext
    {
        #region Properties

        public IEnumerable<Uri> Files { get; }

        #endregion
    }
}