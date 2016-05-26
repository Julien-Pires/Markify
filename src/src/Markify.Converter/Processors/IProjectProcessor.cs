using static Markify.Models.Context;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IProjectProcessor
    {
        #region Methods

        LibraryDefinition Process(ProjectContext project);

        #endregion
    }
}