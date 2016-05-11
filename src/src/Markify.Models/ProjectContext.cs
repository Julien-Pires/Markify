using System;
using System.Collections.Generic;

namespace Markify.Models
{
    public class ProjectContext
    {
        #region Properties

        public virtual IEnumerable<Uri> Files { get; }

        #endregion
    }
}