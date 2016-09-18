using System;
using System.Collections.Generic;
using Markify.Models.IDE;
using Markify.Models.Documents;

namespace Markify.Services.Processing
{
    internal interface IProjectProcessor
    {
        #region Methods

        TableOfContent Process(IEnumerable<Project> projects, Uri root);

        #endregion
    }
}