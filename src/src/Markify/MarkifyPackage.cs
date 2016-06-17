using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Markify.Roslyn;
using Markify.Document;
using Markify.Rendering;
using Markify.Controllers;
using Markify.Core.IDE;
using Markify.Core.IDE.VisualStudio;
using Markify.Rendering.T4;
using Markify.Services;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using Ninject;
using Ninject.Modules;

using EnvDTE;
using EnvDTE80;

namespace Markify
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#2110", "#2112", "1.0", IconResourceID = 2400)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
    public sealed class MarkifyPackage : Package
    {
        #region Fields

        public const string PackageGuidString = "b49bea0e-0ec6-4dd0-bd62-fa1d2932374e";

        private readonly INinjectModule[] _modules =
        {
            new RoslynModule(),
            new DocumentOrganizerModule(),
            new RenderingModule(),
            new T4Module(),
            new IDEModule(),
            new VSModule(GetVisualStudioEnvironment),
            new ServicesModule()
        };

        #endregion

        #region Properties

        internal CommandsController Commands { get; private set; }

        #endregion

        #region Package Members

        protected override void Initialize()
        {
            base.Initialize();

            IKernel kernel = new StandardKernel(_modules);
            Commands = kernel.Get<CommandsController>();
        }

        private static DTE2 GetVisualStudioEnvironment() => GetGlobalService(typeof(DTE)) as DTE2;

        #endregion
    }
}
