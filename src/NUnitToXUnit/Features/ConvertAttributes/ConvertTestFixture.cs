// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features.ConvertAttributes
{
    static class ConvertTestFixture
    {
        public static AttributeListSyntax Convert(AttributeListSyntax node, RequiredUsings requires)
        {
            var testFixture = node.Attributes
                .SingleOrDefault(attribute => attribute.IsTestFixtureAttribute());

            // no test fixture, this isn't an NUnit class
            if (testFixture == null)
            {
                return node;
            }

            requires.XUnit = true;

            // only one attribute in the list (e.g. [TestFixture] ), remove the entire list.
            if (node.Attributes.Count == 1)
            {
                return NullAttributeList(node);
            }

            // multiple attributes in a list (e.g. [TestFixture, DataContract] ), remove only the TestFixture attribute.
            var newList = node.RemoveNode(testFixture, SyntaxRemoveOptions.KeepExteriorTrivia);
            return newList;
        }

        /// <summary>
        /// Return a "null attribute list" that only has trivia, no actual syntax.
        /// This is used when we want to remove an attribute list but keep nearby comments
        /// (e.g. the class documentation comments that can appear above an attribute).
        /// If we just used "null" we would lose those comments.
        /// </summary>
        private static AttributeListSyntax NullAttributeList(AttributeListSyntax node)
        {
            return AttributeList(
                MissingToken(SyntaxKind.OpenBracketToken),
                null, new SeparatedSyntaxList<AttributeSyntax>(),
                MissingToken(SyntaxKind.CloseBracketToken)
            )
            // get rid of the indentation that would appear between the attribute list and the
            // doc comments, but we still want the indentation before the doc comments.
            .WithLeadingTrivia(
                node
                    .GetLeadingTrivia()
                    .Reverse()
                    .SkipWhile(t => t.Kind() == SyntaxKind.WhitespaceTrivia)
                    .Reverse()
            )
            .WithTrailingTrivia(
                node
                    .GetTrailingTrivia()
                    .Where(t => t.Kind() != SyntaxKind.EndOfLineTrivia)
            );
        }
    }
}
