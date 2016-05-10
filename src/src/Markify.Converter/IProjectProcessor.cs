using Markify.Models;

namespace Markify.Processors
{
    public interface IProjectProcessor
    {
        #region Methods

        LibraryDefinition Process(ProjectContext project);

        #endregion
    }
}