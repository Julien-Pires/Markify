using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.Domain.Compiler;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class PropertiesBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly int _count;
        private readonly IEnumerable<string> _visibility;
        private readonly string _set;
        private readonly string _get;

        #endregion

        #region Constructors

        public PropertiesBuilder(int count, IEnumerable<string> visiblity, string set = null,
            string get = null)
        {
            _count = count;
            _visibility = visiblity ?? new string[0];
            _set = set;
            _get = get;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(IEnumerable<PropertyDefinition>) ||
                parameterInfo.Name != "properties")
                return new NoSpecimen();

            return _visibility.SelectMany(c =>
            {
                return Enumerable.Range(0, _count).Select(d => PropertyFactory.Create(c, _set, _get));
            });
        }

        #endregion
    }
}