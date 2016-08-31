using System.Diagnostics.CodeAnalysis;
using Markify.Core.IDE;
using Markify.Core.Rendering;
using Markify.Services.IO;
using Markify.Services.Settings;
using Markify.Services.Processing;
using Ninject.Modules;


namespace Markify.Services
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
            Bind<ISolutionExplorerFilterProvider>().To<SolutionExplorerFilterProvider>();
        }

        #endregion
    }
}