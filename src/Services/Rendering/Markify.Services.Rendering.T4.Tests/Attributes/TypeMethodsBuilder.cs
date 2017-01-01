using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.Domain.Compiler;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class TypeMethodsBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _count;
        private readonly IEnumerable<string> _visibility;

        #endregion

        #region Constructors

        public TypeMethodsBuilder(int count, IEnumerable<string> visiblity)
        {
            _count = count;
            _visibility = visiblity;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(IEnumerable<DelegateDefinition>))
                return new NoSpecimen();

            return _visibility.SelectMany(c =>
            {
                return Enumerable
                    .Range(0, _count)
                    .Select(d =>
                    {
                        var identity = new TypeIdentity(
                            Guid.NewGuid().ToString(),
                            FSharpOption<string>.None,
                            FSharpOption<string>.None,
                            new[] {c},
                            Enumerable.Empty<string>(),
                            Enumerable.Empty<string>(),
                            Enumerable.Empty<GenericParameterDefinition>());

                        return new DelegateDefinition(identity, Enumerable.Empty<ParameterDefinition>(), string.Empty);
                    });
            });
        }

        #endregion
    }
}