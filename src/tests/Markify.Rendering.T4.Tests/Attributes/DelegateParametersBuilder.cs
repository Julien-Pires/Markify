using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using Markify.Models.Definitions;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Rendering.T4.Tests.Attributes
{
    internal sealed class DelegateParametersBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _parametersCount;

        #endregion

        #region Constructors

        public DelegateParametersBuilder(int parametersCount)
        {
            _parametersCount = parametersCount;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(IEnumerable<ParameterDefinition>))
                return new NoSpecimen();

            return Enumerable
                .Range(0, _parametersCount)
                .Select(c => new ParameterDefinition(
                    Guid.NewGuid().ToString(),
                    "int",
                    FSharpOption<string>.None,
                    FSharpOption<string>.None));
        }

        #endregion
    }
}
