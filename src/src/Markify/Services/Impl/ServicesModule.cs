using Ninject.Modules;

namespace Markify.Services.Impl
{
    internal sealed class ServicesModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IProjectProcessor>().To<ProjectProcessor>();
            Bind<IRendererService>().To<RendererService>();
            Bind<IDocumentationGenerator>().To<DocumentationGenerator>();
        }

        #endregion
    }
}