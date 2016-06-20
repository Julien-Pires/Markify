using Markify.Core.IDE;

using static Markify.Models.Context;

namespace Markify.Services.Processing
{
    internal sealed class SolutionExplorerFilterProvider : ISolutionExplorerFilterProvider
    {
        #region Fields

        private SolutionExplorerFilter _filters;

        #endregion

        #region Properties

        public SolutionExplorerFilter Filters
        {
            get
            {
                if (_filters != null)
                    return _filters;

                _filters = new SolutionExplorerFilter(new[] { ProjectLanguage.CSharp });

                return _filters;
            }
        }

        #endregion
    }
}