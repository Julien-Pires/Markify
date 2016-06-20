using Markify.Core.IDE;

using static Markify.Models.Context;

namespace Markify.Services.Processing
{
    internal sealed class SolutionExplorerFilterProvider : ISolutionExplorerFilterProvider
    {
        #region Methods

        public SolutionExplorerFilter GetFilterRules()
        {
            return new SolutionExplorerFilter(new[] { ProjectLanguage.CSharp });
        }

        #endregion
    }
}