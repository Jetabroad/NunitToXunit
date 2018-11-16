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
    static class ConvertAttributes
    {
        public static AttributeSyntax Convert(AttributeSyntax node) =>
            ReplaceAttribute(node).NormalizeWhitespace();

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
    }
}
