﻿using System.Collections.Generic;

using static Markify.Models.Context;

namespace Markify.Services.Impl
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

        public bool Generate(IEnumerable<Project> projects)
        {
            if(projects == null)
                return false;

            var tableOfContent = _projectProcessor.Process(projects);

            return _rendererService.Render(tableOfContent);
        }

        #endregion
    }
}