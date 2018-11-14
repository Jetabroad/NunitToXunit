// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

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
        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            return base.VisitAttribute(ReplaceAttribute(node).NormalizeWhitespace());
        }

        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            var testFixture = node.Attributes
                .SingleOrDefault(attribute => attribute.IsTestFixtureAttribute());

            // no test fixture, this isn't an NUnit class
            if (testFixture == null)
            {
                return base.VisitAttributeList(node);
            }

            requires.XUnit = true;

            // only one attribute in the list (e.g. [TestFixture] ), remove the entire list.
            if (node.Attributes.Count == 1)
            {
                return NullAttributeList(node);
            }

            // multiple attributes in a list (e.g. [TestFixture, DataContract] ), remove only the TestFixture attribute.
            var newList = node.RemoveNode(testFixture, SyntaxRemoveOptions.KeepExteriorTrivia);
            return base.VisitAttributeList(newList);
        }

        private static AttributeSyntax ReplaceAttribute(AttributeSyntax node)
        {
            switch (node.Name.ToString())
            {
                case "Test":
                    return ReplaceByTestOrTheory(node);
                case "TestCaseSource":
                    return ReplaceTestCaseSourceByMemberData(node);
                case "Category":
                    return ReplaceCategoryByTrait(node);
                case "TestCase":
                    return ReplaceTestCaseByInlineData(node);
                default:
                    return node;
            }
        }

        private static AttributeSyntax ReplaceByTestOrTheory(AttributeSyntax node)
        {
            var testMethod = node.Ancestors().SingleOrDefault(x => x is MethodDeclarationSyntax) as MethodDeclarationSyntax;
            if (testMethod == null) return node;
            return testMethod.HasParameter() ? Attribute(ParseName("Xunit.Theory")) : Attribute(ParseName("Xunit.Fact"));
        }

        private static AttributeSyntax ReplaceTestCaseSourceByMemberData(AttributeSyntax node)
        {
            return Attribute(ParseName("Xunit.MemberData"), node.ArgumentList);
        }

        private static AttributeSyntax ReplaceCategoryByTrait(AttributeSyntax node)
        {
            var newArgumentList = CreateTraitArguments(node);
            return Attribute(ParseName("Xunit.Trait"), newArgumentList);
        }

        private static AttributeArgumentListSyntax CreateTraitArguments(AttributeSyntax node)
        {
            var firstArgument = AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Category")));
            var newArguments = node.ArgumentList.Arguments.Insert(0, firstArgument);
            return node.ArgumentList.WithArguments(newArguments);
        }

        private static AttributeSyntax ReplaceTestCaseByInlineData(AttributeSyntax node)
        {
            return Attribute(ParseName("Xunit.InlineData"), node.ArgumentList);
        }

        /// <summary>
        /// Return a "null attribute list" that only has trivia, no actual syntax.
        /// This is used when we want to remove an attribute list but keep nearby comments
        /// (e.g. the class documentation comments that can appear above an attribute).
        /// If we just used "null" we would lose those comments.
        /// </summary>
        private static SyntaxNode NullAttributeList(AttributeListSyntax node)
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
