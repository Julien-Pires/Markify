﻿using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Core.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class VisualStudioEnvironmentDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public VisualStudioEnvironmentDataAttribute(string name, int projectCount, int solutionFolder, params object[] values)
            : this(new VisualStudioEnvironmentCustomization(name, projectCount, solutionFolder), values)
        {
        }

        private VisualStudioEnvironmentDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion
    }
}