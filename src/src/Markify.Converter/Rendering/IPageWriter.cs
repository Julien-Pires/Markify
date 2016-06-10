using System;

using static Markify.Models.Document;

namespace Markify.Core.Rendering
{
    public interface IPageWriter
    {
        #region Methods

        void Write(string text, Page page, Uri root);

        #endregion
    }
}
