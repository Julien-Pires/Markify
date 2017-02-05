using System.Diagnostics.CodeAnalysis;
using Markify.Domain.Compiler;
using Markify.Services.Rendering;
using Markify.Services.Rendering.T4;
using Markify.Services.Rendering.T4.Templates;
using Ninject.Modules;
using TemplateTuple = System.Tuple<Markify.Services.Rendering.T4.T4TemplateBase, System.Type>;

namespace Markify.Application.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class T4Module : NinjectModule
    {
        #region Fields

        private static readonly TemplateTuple[] Templates =
        {
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition.Class)),
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition.Struct)),
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition.Interface)),
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition.Enum)),
            new TemplateTuple(new TypeTemplate(), typeof(TypeDefinition.Delegate))
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