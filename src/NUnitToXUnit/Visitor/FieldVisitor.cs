// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // When we have a parameterized test (some tests have as parameter a field)
            // xUnit demands that the referenced field to be public
            return IsFieldReferencedByTest(node) ? ReplaceModifierFromPrivateToPublic(node) :
                                                   base.VisitFieldDeclaration(node);
        }

        private static bool IsFieldReferencedByTest(FieldDeclarationSyntax field)
        {
            var identifier = field.GetNodeIdentifier();
            var testCaseAttributes = GetTestCaseSourceAttributes(field);

            return testCaseAttributes.Any(attribute => IsIdentifierReferencedByTest(attribute, identifier));
        }

        private static IEnumerable<AttributeListSyntax> GetTestCaseSourceAttributes(FieldDeclarationSyntax field)
        {
            var allMethods = field.Parent.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var allAttributes = allMethods.SelectMany(x => x.AttributeLists);

            return allAttributes.Where(x => x.Attributes.Any(attribute => attribute.IsTestCaseSourceAttribute()));
        }

        private static bool IsIdentifierReferencedByTest(AttributeListSyntax attributeList, string identifier)
        {
            var testCaseAttribute = attributeList.Attributes;
            var argumentList = testCaseAttribute.GetAttributeArguments();

            return argumentList.Any(x => x.Expression.ToString() == $"\"{identifier}\"");
        }

        private static FieldDeclarationSyntax ReplaceModifierFromPrivateToPublic(FieldDeclarationSyntax field)
        {
            var privateModifier = field.Modifiers.Single(x => x.IsPrivate());

            return field.ReplaceToken(privateModifier, Token(SyntaxKind.PublicKeyword));
        }
    }
}
