using System;

using Microsoft.VisualStudio.TextTemplating;

namespace Markify.Rendering.T4
{
    internal sealed class T4Template : ITemplate
    {
        #region Fields

        private readonly TextTransformation _template;

        #endregion

        #region Constructors

        public T4Template(TextTransformation template)
        {
            _template = template;
        }

        #endregion

        #region Methods

        public string Apply(object content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}