using System.Collections.Generic;

namespace Markify.Models.Definitions
{
    public sealed class LibraryDefinition
    {
        #region Properties

        public IEnumerable<TypeDefinition> Types { get; } 

        #endregion
    }
}