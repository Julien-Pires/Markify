using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.CodeAnalyzer;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class TypeFieldsBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _count;
        private readonly IEnumerable<string> _visibility;

        #endregion

        #region Constructors

        public TypeFieldsBuilder(int count, IEnumerable<string> visiblity)
        {
            _count = count;
            _visibility = visiblity ?? new string[0];
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if(parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(IEnumerable<FieldDefinition>))
                return new NoSpecimen();
            
            return _visibility.SelectMany(c =>
            {
                return Enumerable
                    .Range(0, _count)
                    .Select(d => new FieldDefinition(
                        Guid.NewGuid().ToString(),
                        "int",
                        new[] { c },
                        Enumerable.Empty<string>(),
                        FSharpOption<string>.None));
            });
        }

        #endregion
    }
}