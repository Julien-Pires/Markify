using Ninject.Modules;

using TemplateTuple = System.Tuple<Microsoft.VisualStudio.TextTemplating.TextTransformation, System.Type>;

namespace Markify.Rendering.T4
{
    public sealed class T4Module : NinjectModule
    {
        #region Fields

        private static readonly TemplateTuple[] Templates = {};

        #endregion

        #region Module Loading

        public override void Load()
        {
            Bind<ITemplatesProvider>().ToMethod(c => new T4TemplateProvider(Templates));
        }

        #endregion
    }
}