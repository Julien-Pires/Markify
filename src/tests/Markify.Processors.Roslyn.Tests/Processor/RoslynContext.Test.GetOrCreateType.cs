using System;
using Markify.Models.Definitions;
using Markify.Processors.Roslyn.Models;

using Xunit;

namespace Markify.Processors.Roslyn.Tests
{
    public partial class RoslynContext_Test
    {
        [Fact]
        public void GetOrCreateType_WhenDoesNotExist_WithSuccess()
        {
            RoslynContext context = new RoslynContext();

            int countBefore = context.Types.Count;
            TypeRepresentation type = context.GetOrCreateType(StructureKind.Class, "Test", "Test");

            Assert.Equal(0, countBefore);
            Assert.Equal(1, context.Types.Count);
            Assert.NotNull(type);
        }

        [Fact]
        public void GetOrCreateType_WhenTypeExist_WithSuccessAndNoDuplicate()
        {
            RoslynContext context = new RoslynContext();
            TypeRepresentation initialType = context.GetOrCreateType(StructureKind.Class, "Test");

            int countBefore = context.Types.Count;
            TypeRepresentation type = context.GetOrCreateType(StructureKind.Class, "Test");

            Assert.Equal(1, countBefore);
            Assert.Equal(1, context.Types.Count);
            Assert.NotNull(type);
            Assert.Equal(initialType, type);
        }

        [Theory]
        [InlineData("Foo", "Foo")]
        [InlineData("Foo.Bar", "Bar")]
        [InlineData("Foo.Bar.FooBar", "FooBar")]
        public void GetOrCreateType_WhenPassingOnlyFullname_WithSuccessAndCorrectName(string fullname, string name)
        {
            RoslynContext context = new RoslynContext();

            TypeRepresentation type = context.GetOrCreateType(StructureKind.Class, fullname);

            Assert.NotNull(type);
            Assert.Equal(name, type.Name);
        }

        [Theory]
        [InlineData("Foo.Bar", "Bar", StructureKind.Class)]
        [InlineData("Foo.FooBar.IBar", "IBar", StructureKind.Interface)]
        public void GetOrCreateType_WhenPassingParameters_WithCorrectParameters(string fullname, string name, StructureKind structure)
        {
            RoslynContext context = new RoslynContext();

            TypeRepresentation type = context.GetOrCreateType(structure, fullname, name);

            Assert.NotNull(type);
            Assert.Equal(fullname, type.Fullname);
            Assert.Equal(name, type.Name);
            Assert.Equal(structure, type.Structure);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void GetOrCreateType_WhenInvalidFullname_ThrowException(string fullname)
        {
            RoslynContext context = new RoslynContext();

            Assert.Throws<ArgumentNullException>(() => context.GetOrCreateType(StructureKind.Class, fullname, "Bar"));
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void GetOrCreateType_WhenInvalidStructureLKind_ThrowException(int structure)
        {
            RoslynContext context = new RoslynContext();

            Assert.Throws<ArgumentOutOfRangeException>(() => context.GetOrCreateType((StructureKind)structure, "Foo.Bar", "Bar"));
        }

        [Theory]
        [InlineData("Foo..Bar")]
        [InlineData("Foo.   .Bar")]
        [InlineData("Foo.Bar.")]
        [InlineData("Foo.Bar.  ")]
        [InlineData(".Bar")]
        [InlineData(".Bar..FooBar")]
        [InlineData(".Bar.   ..Foobar")]
        public void GetOrCreateType_WhenFullnameHasInvalidPart_ThrowException(string fullname)
        {
            RoslynContext context = new RoslynContext();

            Assert.Throws<ArgumentException>(() => context.GetOrCreateType(StructureKind.Class, fullname));
        }
    }
}