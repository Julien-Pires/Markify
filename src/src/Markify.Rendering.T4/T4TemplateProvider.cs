using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Optional;

using TemplateTuple = System.Tuple<Markify.Rendering.T4.T4TemplateBase, System.Type>;

namespace Markify.Rendering.T4
{
    public sealed class T4TemplateProvider : ITemplatesProvider
    {
        #region Fields

        private readonly IImmutableDictionary<Type, ITemplate> _templates;

        #endregion

        #region Constructors

        public T4TemplateProvider(IEnumerable<TemplateTuple> templates)
        {
            _templates = templates.Where(c => c != null)
                                  .ToImmutableDictionary(c => c.Item2, c => (ITemplate)new T4Template(c.Item1));
        }

        #endregion

        #region Methods

        public Option<ITemplate> GetTemplate(object content)
        {
            if (content == null)
                return Option.None<ITemplate>();

            ITemplate result;
            if (!_templates.TryGetValue(content.GetType(), out result))
                return Option.None<ITemplate>();

            return Option.Some(result);
        }

        #endregion
    }
}