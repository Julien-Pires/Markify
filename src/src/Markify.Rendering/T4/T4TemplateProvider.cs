using System;
using System.Linq;
using System.Collections.Generic;

using Optional;

using TemplateTuple = System.Tuple<Microsoft.VisualStudio.TextTemplating.TextTransformation, System.Type>;

namespace Markify.Rendering.T4
{
    internal sealed class T4TemplateProvider : ITemplatesProvider
    {
        #region Fields

        private readonly Dictionary<Type, ITemplate> _templates = new Dictionary<Type, ITemplate>();

        #endregion

        #region Constructors

        internal T4TemplateProvider(IEnumerable<TemplateTuple> templates)
        {
            //_templates = templates.ToDictionary(c => c.Item2, c => (ITemplate)new T4Template(c.Item1));
        }

        #endregion

        #region Methods

        public Option<ITemplate> GetTemplate(object content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}