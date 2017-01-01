using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Markify.Application.Commands;
using Markify.Application.Controllers;
using Markify.Application.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;

namespace Markify.Application
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#2110", "#2112", "1.0", IconResourceID = 2400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
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
            new DocumentationOrganizerModule(),
            new RenderingModule(),
            new T4Module(),
            new VisualStudioModule(GetVisualStudioEnvironment),
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

            var packageArg = new ConstructorArgument("package", this);
            Func<Type, object> cmdFactory = c => kernel.Get(c, packageArg);
            GenerateSolutionDocumentationCommand.Initialize(cmdFactory);
            GenerateCurrentProjectCommand.Initialize(cmdFactory);
        }

        private static DTE2 GetVisualStudioEnvironment() => GetGlobalService(typeof(DTE)) as DTE2;

        #endregion
    }
}
