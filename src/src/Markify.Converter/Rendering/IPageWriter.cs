using System;

using Markify.Models.Documents;

namespace Markify.Core.Rendering
{
    public interface IPageWriter
    {
        #region Methods

        void Write(string text, Page page, Uri root);

        #endregion
    }
}
