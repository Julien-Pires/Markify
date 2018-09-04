using System;
using System.Collections.Generic;
using Markify.Domain.Ide;

namespace Markify.Application.Services.Processing
{
    internal class DocumentationGenerator : IDocumentationGenerator
    {
        #region Fields

        private readonly IProjectProcessor _projectProcessor;
        private readonly IRenderer _rendererService;

        #endregion

        #region Constructors

        public DocumentationGenerator(IProjectProcessor processor, IRenderer renderer)
        {
            _projectProcessor = processor;
            _rendererService = renderer;
        }

        #endregion

        #region Generator Methods

        public bool Generate(IEnumerable<Project> projects, Uri root)
        {
            if(projects == null)
            {
                return false;
            }

            var tableOfContent = _projectProcessor.Process(projects, root);

            return _rendererService.Render(tableOfContent);
        }

        #endregion
    }
}