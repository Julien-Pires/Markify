using Markify.Application.Services;
using Markify.Domain.Ide;
using static Markify.Core.FSharp.FSharpOptionExtension;

namespace Markify.Application.Controllers
{
    internal class CommandsController
    {
        #region Fields

        private readonly IIdeEnvironment _ide;
        private readonly IDocumentationGenerator _generator;

        #endregion

        #region Constructors

        public CommandsController(IIdeEnvironment ide, IDocumentationGenerator generator)
        {
            _ide = ide;
            _generator = generator;
        }

        #endregion

        #region Commands Actions

        public bool GenerateForCurrentProject()
        {
            return _ide.CurrentProject.Match(
                c =>
                {
                    var solution = _ide.CurrentSolution;
                    var root = solution.Match(
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
            return _ide.CurrentSolution.Match(
                c => _generator.Generate(c.Projects, c.Path),
                () => false
            );
        }

        #endregion
    }
}