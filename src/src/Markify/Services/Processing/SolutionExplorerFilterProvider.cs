using Markify.Core.IDE;

using Markify.Models.IDE;

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

                _filters = new SolutionExplorerFilter(new[] { ProjectLanguage.CSharp }, new []{ ".cs" });

                return _filters;
            }
        }

        #endregion
    }
}