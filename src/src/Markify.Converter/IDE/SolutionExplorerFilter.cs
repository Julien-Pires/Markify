using System.Collections.Generic;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public sealed class SolutionExplorerFilter
    {
        #region Properties

        public IEnumerable<ProjectLanguage> SupportedLanguages { get; }

        #endregion

        #region Constructors

        public SolutionExplorerFilter(IEnumerable<ProjectLanguage> languages)
        {
            SupportedLanguages = languages;
        }

        #endregion
    }
}