using System;
using Markify.Domain.Document;
using Markify.Domain.Rendering;

namespace Markify.Services.Rendering.Tests.Attributes
{
    internal sealed class FakePageWriter : IPageWriter
    {
        #region Properties

        public int Written { get; private set; }

        #endregion

        #region Methods

        public void Write(string text, Page page, Uri root)
        {
            Written++;
        }

        #endregion
    }
}