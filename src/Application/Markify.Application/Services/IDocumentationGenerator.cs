using System;
using System.Collections.Generic;
using Markify.Domain.Ide;

namespace Markify.Application.Services
{
    internal interface IDocumentationGenerator
    {
        #region Methods

        bool Generate(IEnumerable<Project> projects, Uri root);

        #endregion
    }
}