using Markify.Services.Settings;
using Markify.Services.Processing;

using Ninject.Modules;

namespace Markify.Services
{
    internal sealed class ServicesModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IProjectProcessor>().To<ProjectProcessor>();
            Bind<IRendererService>().To<RendererService>();
            Bind<IDocumentationGenerator>().To<DocumentationGenerator>();
            Bind<ISettingsProvider>().To<SettingsProvider>();
        }

        #endregion
    }
}