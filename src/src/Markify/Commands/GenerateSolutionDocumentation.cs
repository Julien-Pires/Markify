using System;
using System.ComponentModel.Design;

using Microsoft.VisualStudio.Shell;

using Markify.Controllers;

namespace Markify.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal class GenerateSolutionDocumentationCommand
    {
        #region Fields

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c8854178-213c-4435-995d-5003ad4e545a");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private readonly CommandsController _controller;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateSolutionDocumentationCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateSolutionDocumentationCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="controller">Controller that contains menu command</param>
        public GenerateSolutionDocumentationCommand(Package package, CommandsController controller)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            _package = package;
            _controller = controller;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        public static void Initialize(Func<Type, object> factory)
        {
            Instance = (GenerateSolutionDocumentationCommand)factory(typeof(GenerateSolutionDocumentationCommand));
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            _controller.GenerateForCurrentSolution();
        }

        #endregion
    }
}