using System.Diagnostics.CodeAnalysis;
using Markify.Core.Rendering;
using Ninject.Modules;

namespace Markify.Rendering
{
    [ExcludeFromCodeCoverage]
    public sealed class RenderingModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IDocumentRenderer>().To<PagesRenderer>();
        }

        #endregion
    }
}