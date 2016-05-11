using System;
using Markify.Models;
using Markify.Models.Definitions;

namespace Markify.Processors.Roslyn
{
    public class RoslynProcessorToto : IProjectProcessor
    {
        #region Processing

        public AssemblyDefinition Process()
        {
            return null;
        }

        public LibraryDefinition Process(ProjectContext project)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}