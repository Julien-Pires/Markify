using System.Diagnostics.CodeAnalysis;
using Markify.Domain.Rendering;
using Markify.Services.Rendering;
using Ninject.Modules;

namespace Markify.Application.Services
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