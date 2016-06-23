using Markify.Rendering.T4.Templates;

using Markify.Models.Definitions;

using Ninject.Modules;

using TemplateTuple = System.Tuple<Markify.Rendering.T4.T4TemplateBase, System.Type>;

namespace Markify.Rendering.T4
{
    public sealed class T4Module : NinjectModule
    {
        #region Fields

        private static readonly TemplateTuple[] Templates =
        {
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition)), 
        };

        #endregion

        #region Module Loading

        public override void Load()
        {
            Bind<ITemplatesProvider>().ToMethod(c => new T4TemplateProvider(Templates));
        }

        #endregion
    }
}