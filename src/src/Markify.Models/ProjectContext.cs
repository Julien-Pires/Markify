using System;
using System.Collections.ObjectModel;

namespace Markify.Models
{
    public sealed class ProjectContext
    {
        #region Properties

        public ReadOnlyCollection<Uri> Files;

        #endregion

        #region Methods

        public void AddFile(Uri path)
        {
        }

        #endregion
    }
}