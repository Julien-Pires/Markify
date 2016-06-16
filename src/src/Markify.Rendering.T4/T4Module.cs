using Ninject.Modules;

using TemplateTuple = System.Tuple<Microsoft.VisualStudio.TextTemplating.TextTransformation, System.Type>;

namespace Markify.Rendering.T4
{
    public sealed class T4Module : NinjectModule
    {
        #region Fields

        private readonly TemplateTuple[] _templates = {};

        #endregion

        #region Module Loading

        public override void Load()
        {
            Bind<ITemplatesProvider>().To<T4TemplateProvider>()
                                      .WithConstructorArgument(_templates);
        }

        #endregion
    }
}