using System.Collections.Generic;

using Markify.Models.IDE;

namespace Markify.Services.Processing
{
    internal class DocumentationGenerator : IDocumentationGenerator
    {
        #region Fields

        private readonly IProjectProcessor _projectProcessor;
        private readonly IRendererService _rendererService;

        #endregion

        #region Constructors

        public DocumentationGenerator(IProjectProcessor processor, IRendererService renderer)
        {
            _projectProcessor = processor;
            _rendererService = renderer;
        }

        #endregion

        #region Generator Methods

        public bool Generate(IEnumerable<Project> projects, Solution solution)
        {
            if(projects == null)
                return false;

            var tableOfContent = _projectProcessor.Process(projects, solution);

            return _rendererService.Render(tableOfContent);
        }

        #endregion
    }
}