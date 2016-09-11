using System;
using Markify.Core.Rendering;
using Markify.Models.Documents;

namespace Markify.Rendering.Tests.Attributes
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