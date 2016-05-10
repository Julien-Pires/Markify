using System.Collections.Generic;

using Markify.Models.Definitions;

namespace Markify.Models
{
    public sealed class LibraryDefinition
    {
        #region Properties

        public IEnumerable<AssemblyDefinition> Assemblies { get; }

        public IEnumerable<TypeDefinition> Types { get; } 

        #endregion
    }
}