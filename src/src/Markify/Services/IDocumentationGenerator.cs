using System;
using System.Collections.Generic;
using Markify.Models.IDE;

namespace Markify.Services
{
    internal interface IDocumentationGenerator
    {
        #region Methods

        bool Generate(IEnumerable<Project> projects, Uri root);

        #endregion
    }
}