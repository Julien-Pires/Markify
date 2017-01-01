using System.Diagnostics.CodeAnalysis;
using Markify.Application.Services.IO;
using Markify.Application.Services.Processing;
using Markify.Application.Services.Settings;
using Markify.Domain.Ide;
using Markify.Domain.Rendering;
using Ninject.Modules;

namespace Markify.Application.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class ServicesModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IPageWriter>().To<PageWriter>();
            Bind<IProjectProcessor>().To<ProjectProcessor>();
            Bind<IRenderer>().To<Renderer>();
            Bind<IDocumentationGenerator>().To<DocumentationGenerator>();
            Bind<IDocumentSettingsProvider>().To<DocumentSettingsProvider>();
            Bind<IProjectFilterProvider>().To<SolutionExplorerFilterProvider>();
        }

        #endregion
    }
}