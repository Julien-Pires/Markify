using System.Collections.Generic;
using System.Linq;
using Markify.CodeAnalyzer;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        #region Helpers

        private static IEnumerable<PropertyDefinition> GetProperties(TypeDefinition definition)
        {
            IEnumerable<PropertyDefinition> properties;
            switch (definition.Tag)
            {
                case TypeDefinition.Tags.Class:
                    properties = ((TypeDefinition.Class) definition).Item.Properties;
                    break;

                case TypeDefinition.Tags.Struct:
                    properties = ((TypeDefinition.Struct)definition).Item.Properties;
                    break;

                case TypeDefinition.Tags.Interface:
                    properties = ((TypeDefinition.Interface)definition).Item.Properties;
                    break;

                default:
                    properties = Enumerable.Empty<PropertyDefinition>();
                    break;
            }

            return properties;
        }

        #endregion
    }
}