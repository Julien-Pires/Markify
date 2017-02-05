using System;
using Moq;
using Ploeh.AutoFixture;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    public class TemplateCustomization : ICustomization
    {
        #region Fields

        private readonly string _value;
        private readonly bool _mustThrow;

        #endregion

        #region Constructors

        public TemplateCustomization(string value, bool mustThrow)
        {
            _value = value;
            _mustThrow = mustThrow;
        }

        #endregion

        #region Customization

        public static T4TemplateBase CreateTemplate(string value, bool mustThrow)
        {
            var template = new Mock<T4TemplateBase>();
            template.SetupAllProperties();

            var transformSetup = template.Setup(c => c.TransformText());
            if (!mustThrow)
            {
                transformSetup.Returns(() =>
                {
                    var self = template.Object;
                    var content = self.Session["Content"];

                    return content != null ? $"{value}-{content}" : value;
                });
            }
            else
                transformSetup.Returns(() => { throw new InvalidOperationException(); });


            return template.Object;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(new T4Template(CreateTemplate(_value, _mustThrow)));
        }

        #endregion
    }
}