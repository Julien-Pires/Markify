using System.Reflection;
using Markify.Domain.Compiler;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class PropertyBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly string _visibility;
        private readonly string _set;
        private readonly string _get;

        #endregion

        #region Constructors

        public PropertyBuilder(string visiblity, string set, string get)
        {
            _visibility = visiblity;
            _set = set;
            _get = get;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if(parameterInfo == null)
                return new NoSpecimen();

            if(parameterInfo.ParameterType != typeof(PropertyDefinition))
                return new NoSpecimen();

            return PropertyFactory.Create(_visibility, _set, _get);
        }

        #endregion
    }
}