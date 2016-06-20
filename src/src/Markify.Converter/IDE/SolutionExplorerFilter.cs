using System.Collections.Generic;
using System.Collections.Immutable;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public sealed class SolutionExplorerFilter
    {
        #region Properties

        public IEnumerable<ProjectLanguage> SupportedLanguages { get; }

        public IEnumerable<string> SupportedFiles { get; }

        #endregion

        #region Constructors

        public SolutionExplorerFilter(IEnumerable<ProjectLanguage> languages, IEnumerable<string> files)
        {
            SupportedLanguages = languages?.ToImmutableList() ?? ImmutableList.Create<ProjectLanguage>();
            SupportedFiles = files?.ToImmutableList() ?? ImmutableList.Create<string>();
        }

        #endregion
    }
}