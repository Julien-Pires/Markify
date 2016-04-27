using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Extensions;

namespace Markify.Processors.Roslyn.Inspectors
{
    public class GenericParameterInspector : ISyntaxTreeInspector<GenericParameterRepresentation>
    {
        #region Inspect

        public IEnumerable<GenericParameterRepresentation> Inspect(SyntaxNode node)
        {
            if(node == null)
                throw new ArgumentNullException(nameof(node));

            ITypeDeclarationAdapter typeAdapter = TypeDeclarationAdapterFactory.Create(node);
            if (typeAdapter == null)
                throw new ArgumentException($"{node} is not a valid generic node");

            List<GenericParameterRepresentation> genericParameters = new List<GenericParameterRepresentation>();
            if (typeAdapter.ParameterList == null)
                return genericParameters.AsReadOnly();

            foreach (TypeParameterSyntax parameter in typeAdapter.ParameterList.Parameters)
            {
                string name = parameter.Identifier.ToString();
                TypeParameterConstraintClauseSyntax constraintClause = typeAdapter.ConstraintClauses.FirstOrDefault(c => c.Name.ToString() == name);
                ImmutableList<string> constraints = ImmutableList.Create<string>();
                if (constraintClause != null)
                    constraints = constraintClause.Constraints.Aggregate(constraints, (current, constraint) => current.Add(constraint.ToString()));

                Fullname fullname = new Fullname(ImmutableList.Create(name));
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