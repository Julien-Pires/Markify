using System.Collections.Generic;

using static Markify.Models.Document;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IDocumentOrganizer
    {
        #region Methods

        TableOfContent Organize(IEnumerable<LibraryDefinition> library, DocumentSetting setting);

        #endregion
    }
}
