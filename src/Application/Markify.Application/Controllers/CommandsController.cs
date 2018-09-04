using Markify.Application.Services;
using Markify.Domain.Ide;
using static Markify.Core.FSharp.FSharpOptionExtension;

namespace Markify.Application.Controllers
{
    internal class CommandsController
    {
        #region Fields

        private readonly IIDEExplorer _ideExplorer;
        private readonly IDocumentationGenerator _generator;

        #endregion

        #region Constructors

        public CommandsController(IIDEExplorer ideExplorer, IDocumentationGenerator generator)
        {
            _ideExplorer = ideExplorer;
            _generator = generator;
        }

        #endregion

        #region Commands Actions

        public bool GenerateForCurrentProject()
        {
            return _ideExplorer.ActiveProject.Match(
                c =>
                {
                    var root = _ideExplorer.ActiveSolution.Match(
                        d => d.Path,
                        () => c.Path
                    );

                    return _generator.Generate(new[] {c}, root);
                },
                () => false
            );
        }

        public bool GenerateForCurrentSolution()
        {
            return _ideExplorer.ActiveSolution.Match(
                c => _generator.Generate(_ideExplorer.Projects, c.Path),
                () => false
            );
        }

        #endregion
    }
}