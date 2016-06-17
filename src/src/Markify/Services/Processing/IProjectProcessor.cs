using System.Collections.Generic;

using static Markify.Models.Document;
using static Markify.Models.Context;

namespace Markify.Services.Processing
{
    internal interface IProjectProcessor
    {
        #region Methods

        TableOfContent Process(IEnumerable<Project> projects, Solution solution);

        #endregion
    }
}