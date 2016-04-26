using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Markify.Processors.Roslyn.Models;

namespace Markify.Processors.Roslyn.Inspectors
{
    public class GenericParameterInspector : ISyntaxTreeInspector<GenericParameterRepresentation>
    {
        #region Helpers

        private static TypeParameterListSyntax GetParameters(SyntaxNode node)
        {
            TypeDeclarationSyntax type = node as TypeDeclarationSyntax;
            if (type != null)
                return type.TypeParameterList;

            DelegateDeclarationSyntax method = node as DelegateDeclarationSyntax;
            if (method != null)
                return method.TypeParameterList;

            throw new InvalidOperationException($"{node} cannot contains generic parameters");
        }

        private static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraints(SyntaxNode node)
        {
            TypeDeclarationSyntax type = node as TypeDeclarationSyntax;
            if (type != null)
                return type.ConstraintClauses;

            DelegateDeclarationSyntax method = node as DelegateDeclarationSyntax;
            if (method != null)
                return method.ConstraintClauses;

            throw new InvalidOperationException($"{node} cannot contains generic parameters");
        }

        #endregion

        #region Inspect

        public IEnumerable<GenericParameterRepresentation> Inspect(SyntaxNode node)
        {
            if(node == null)
                throw new ArgumentNullException(nameof(node));

            if (!(node is TypeDeclarationSyntax) && !(node is DelegateDeclarationSyntax))
                throw new ArgumentException($"{node} is not a valid generic node");

            TypeParameterListSyntax typeParameters = GetParameters(node);
            List<GenericParameterRepresentation> genericParameters = new List<GenericParameterRepresentation>();
            if (typeParameters == null)
                return genericParameters.AsReadOnly();

            SyntaxList<TypeParameterConstraintClauseSyntax> typeConstraints = GetConstraints(node);
            foreach (var parameter in typeParameters.Parameters)
            {
                string name = parameter.Identifier.ToString();
                TypeParameterConstraintClauseSyntax constraintClause = typeConstraints.FirstOrDefault(c => c.Name.ToString() == name);
                ImmutableArray<string> constraints = ImmutableArray.Create<string>();
                if (constraintClause != null)
                    constraints = constraintClause.Constraints.Aggregate(constraints, (current, constraint) => current.Add(constraint.ToString()));

                Fullname fullname = new Fullname(ImmutableArray.Create(name));
                genericParameters.Add(new GenericParameterRepresentation(fullname)
                {
                    Modifier = parameter.VarianceKeyword.Text,
                    Constraints = constraints
                });
            }

            return genericParameters.AsReadOnly();
        }

        #endregion
    }
}