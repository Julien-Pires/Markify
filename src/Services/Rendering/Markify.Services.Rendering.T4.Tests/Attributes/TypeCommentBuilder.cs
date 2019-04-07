using System;
using System.Linq;
using System.Reflection;
using Markify.CodeAnalyzer;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    public class TypeCommentBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly bool _hasComments;

        #endregion

        #region Constructors

        public TypeCommentBuilder(bool hasComments)
        {
            _hasComments = hasComments;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is ParameterInfo parameterInfo))
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(TypeComments))
                return new NoSpecimen();

            return _hasComments ?
                new TypeComments(
                    new []{
                        new Comment("summary", new[] { CommentContent.NewText(Guid.NewGuid().ToString()) }, new CommentParameter[0]),
                        new Comment("remarks", new[] { CommentContent.NewText(Guid.NewGuid().ToString()) }, new CommentParameter[0]),
                        new Comment("example", new[] { CommentContent.NewText(Guid.NewGuid().ToString()) }, new CommentParameter[0])
                    }
                ) :
                new TypeComments(Enumerable.Empty<Comment>());
        }

        #endregion
    }
}