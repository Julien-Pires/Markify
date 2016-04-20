using Markify.Models.Definitions;

namespace Markify.Processors
{
    public interface IProjectProcessor
    {
        #region Methods

        AssemblyDefinition Process();

        #endregion
    }
}