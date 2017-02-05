using System;
using System.Collections.Generic;
using Markify.Domain.Document;
using Markify.Domain.Ide;

namespace Markify.Application.Services.Processing
{
    internal interface IProjectProcessor
    {
        #region Methods

        TableOfContent Process(IEnumerable<Project> projects, Uri root);

        #endregion
    }
}