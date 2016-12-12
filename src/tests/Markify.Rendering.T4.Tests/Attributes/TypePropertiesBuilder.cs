using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.Models.Definitions;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Rendering.T4.Tests.Attributes
{
    internal sealed class TypePropertiesBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _count;
        private readonly IEnumerable<string> _visibility;
        private readonly string _setAccessor;
        private readonly string _getAccessor;

        #endregion

        #region Constructors

        public TypePropertiesBuilder(int count, IEnumerable<string> visiblity, string setAccessor = null, 
            string getAccessor = null)
        {
            _count = count;
            _visibility = visiblity;
            _setAccessor = setAccessor;
            _getAccessor = getAccessor;
        }

        #endregion

        #region Builder

        private static FSharpOption<AccessorDefinition> CreateAccessor(string visiblity)
        {
            return string.IsNullOrWhiteSpace(visiblity)
                ? FSharpOption<AccessorDefinition>.None
                : FSharpOption<AccessorDefinition>.Some(new AccessorDefinition(new[] {visiblity}));
        }

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if(parameterInfo == null)
                return new NoSpecimen();

            if(parameterInfo.ParameterType != typeof(IEnumerable<PropertyDefinition>) || 
                parameterInfo.Name != "properties")
                return new NoSpecimen();
            
            return _visibility.SelectMany(c =>
            {
                return Enumerable
                    .Range(0, _count)
                    .Select(d => new PropertyDefinition(
                        Guid.NewGuid().ToString(),
                        "int",
                        new[] { c },
                        Enumerable.Empty<string>(),
                        FSharpOption<string>.None,
                        CreateAccessor(_setAccessor),
                        CreateAccessor(_getAccessor)));
            });
        }

        #endregion
    }
}