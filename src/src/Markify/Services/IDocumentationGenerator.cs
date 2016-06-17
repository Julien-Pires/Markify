using System.Collections.Generic;

using static Markify.Models.Context;

namespace Markify.Services
{
    internal interface IDocumentationGenerator
    {
        #region Methods

        bool Generate(IEnumerable<Project> projects, Solution solution);

        #endregion
    }
}