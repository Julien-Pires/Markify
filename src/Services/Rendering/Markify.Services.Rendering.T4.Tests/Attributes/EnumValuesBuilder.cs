using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.CodeAnalyzer;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    public sealed class EnumValuesBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _count;

        #endregion

        #region Constructors

        public EnumValuesBuilder(int count)
        {
            _count = count;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(IEnumerable<EnumValue>) ||
                parameterInfo.Name != "values")
                return new NoSpecimen();

            return Enumerable.Range(0, _count)
                .Select(c => new EnumValue($"Value{c}", FSharpOption<string>.None));
        }

        #endregion
    }
}