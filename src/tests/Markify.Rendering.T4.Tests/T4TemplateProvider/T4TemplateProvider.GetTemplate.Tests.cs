using System;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed class T4TemplateProviderTests
    {
        [Theory]
        [TemplateProviderData(new[] { typeof(string)}, typeof(string))]
        [TemplateProviderData(new[] { typeof(string), typeof(int), typeof(DateTime) }, typeof(DateTime))]
        public void GetTemplate_ShouldReturnTemplateInstance_WhenTypeIsRegistered(object content, T4TemplateProvider sut)
        {
            Check.That(sut.GetTemplate(content).HasValue).IsTrue();
        }

        [Theory]
        [TemplateProviderData(new Type[0], typeof(int))]
        [TemplateProviderData(new[] { typeof(string), typeof(int), typeof(DateTime) }, typeof(float))]
        public void GetTemplate_ShouldReturnNone_WhenTypeIsNotRegistered(object content, T4TemplateProvider sut)
        {
            Check.That(sut.GetTemplate(content).HasValue).IsFalse();
        }

        [Theory]
        [TemplateProviderData(new Type[0], null)]
        public void GetTemplate_ShouldReturnNone_WhenContentIsNull(object content, T4TemplateProvider sut)
        {
            Check.That(sut.GetTemplate(content).HasValue).IsFalse();
        }
    }
}