using Markify.Core.IDE;

using Markify.Models.IDE;

namespace Markify.Services.Settings
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

                _filters = new SolutionExplorerFilter(
                    new[] { ProjectLanguage.CSharp, ProjectLanguage.VisualBasic }, 
                    new[] { ".cs", ".vb" }
                );

                return _filters;
            }
        }

        #endregion
    }
}