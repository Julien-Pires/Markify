using System;
using System.Collections.Generic;
using Markify.Models.Documents;
using Markify.Models.Definitions;

namespace Markify.Core.Analyzers
{
    public interface IDocumentationOrganizer
    {
        #region Methods

        TableOfContent Organize(IEnumerable<LibraryDefinition> library, Uri root, DocumentSetting setting);

        #endregion
    }
}