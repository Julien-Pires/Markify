using Optional;

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
            _template.Session = new TextTemplatingSession();
        }

        #endregion

        #region Methods

        public Option<string> Apply(object content)
        {
            try
            {
                _template.Session["Content"] = content;
                _template.Initialize();

                return _template.TransformText().Some();
            }
            catch
            {
                return Option.None<string>();
            }
            finally
            {
                _template.Session.Clear();
            }
        }

        #endregion
    }
}