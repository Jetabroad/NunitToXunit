// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        private const string NUnitStringCompareIgnoreCase = "IgnoreCase";

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (!_options.ConvertAssert) return node;
            _options.RequiresXUnitImport = true;
            return base.VisitInvocationExpression(ReplaceInvocationExpression(node));
        }

        private static InvocationExpressionSyntax ReplaceInvocationExpression(InvocationExpressionSyntax originalNode)
        {
            var node = ReplaceFailAssert(originalNode);
            node = ReplaceThatAssert(node);
            return ReplaceCompareOperationAssert(node).WithTriviaFrom(originalNode);
        }

        private static InvocationExpressionSyntax ReplaceFailAssert(InvocationExpressionSyntax node) =>
            node.IsFailExpression() ? CreateFailExpression(node) : node;

        private static InvocationExpressionSyntax CreateFailExpression(InvocationExpressionSyntax node)
        {
            // From https://xunit.github.io/docs/comparisons.html:
            // When we find Assert.Fail("message")
            // We want replace by Assert.True(false, "message")
            var falseArgument = Argument(LiteralExpression(SyntaxKind.FalseLiteralExpression));
            var assertArguments = node.ArgumentList.Arguments.Insert(0, falseArgument);
            var expression = CreateAssertMemberAccessExpression("True");
            return InvocationExpression(expression, ArgumentList(assertArguments)).NormalizeWhitespace();
        }

        private static InvocationExpressionSyntax ReplaceThatAssert(InvocationExpressionSyntax node)
        {
            if (!IsAssertWithTwoArguments(node)) return node;

            var actual = node.ArgumentList.Arguments[0];
            var expression = node.ArgumentList.Arguments[1].Expression;

            return ThatAssertionsSingleArgument.ContainsKey(expression.ToString())
                ? CreateSingleArgumentExpression(node, expression, actual)
                : TryReplaceInvocationExpression(node, expression, actual);
        }

        private static InvocationExpressionSyntax CreateSingleArgumentExpression(
            InvocationExpressionSyntax node,
            ExpressionSyntax assertExpression,
            ArgumentSyntax actualArgument)
        {
            return InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("Assert").WithTriviaFrom(node),
                    IdentifierName(ThatAssertionsSingleArgument[assertExpression.ToString()])
                ),
                ArgumentList(SingletonSeparatedList(actualArgument))
            )
            .NormalizeWhitespace();
        }

        private static InvocationExpressionSyntax TryReplaceInvocationExpression(
            InvocationExpressionSyntax node,
            ExpressionSyntax assertExpression,
            ArgumentSyntax actualArgument)
        {
            return ReplaceInvocationExpressionCompareArgument(assertExpression, actualArgument)?.NormalizeWhitespace().WithTriviaFrom(node)
                ?? node;
        }

        /// <summary>
        /// Recursively visit an nunit expression and convert it to an Xunit expression
        /// </summary>
        /// <example>
        ///    Assert.That("Hello World", Is.StringContaining("WORLD").IgnoreCase);
        ///    should be converted to
        ///    Assert.Contains("WORLD", "Hello World", StringComparison.OrdinalIgnoreCase);
        /// </example>
        private static InvocationExpressionSyntax ReplaceInvocationExpressionCompareArgument(
            ExpressionSyntax assertExpression,
            ArgumentSyntax actualArgument)
        {
            switch (assertExpression)
            {
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return ReplaceByXUnitAssertions(memberAccessExpression, actualArgument);
                case InvocationExpressionSyntax invocationExpressionSyntax:
                    return ReplaceInvocationExpressionCompareArgument(invocationExpressionSyntax.Expression, actualArgument);
            }
            return null;
        }

        private static InvocationExpressionSyntax ReplaceByXUnitAssertions(
            MemberAccessExpressionSyntax memberAccessExpression,
            ArgumentSyntax actualArgument)
        {
            var expressionName = GetNUnitExpressionName(memberAccessExpression);
            if (expressionName == NUnitStringCompareIgnoreCase)
            {
                // recursively convert the remainder of the expression, then apply the 'ignore case' argument
                // e.g. Is.StringContaining("WORLD").IgnoreCase will then convert Is.StringContaining("WORLD")
                var convertedExpression = ReplaceInvocationExpressionCompareArgument(memberAccessExpression.Expression, actualArgument);
                return AddXunitIgnoreCase(convertedExpression);
            }

            if (ThatAssertionGenericType.TryGetValue(expressionName, out string replacement))
            {
                var expectedType = GetExpectedType(memberAccessExpression);
                var assert = CreateAssertMemberAccessExpressionWithGenericType(replacement, expectedType);
                return InvocationExpression(assert, ArgumentList(SeparatedList(new[] { actualArgument })));
            }

            var expectedArgument = GetExpectedArgument(memberAccessExpression);
            return expectedArgument != null
                ? ReplaceXUnitAssertionsWithExpectedArgument(expressionName, memberAccessExpression, expectedArgument, actualArgument)
                // We not yet support other cases doesn't have expected argument.
                : null;
        }

        private static InvocationExpressionSyntax AddXunitIgnoreCase(InvocationExpressionSyntax convertedExpression)
        {
            var ignoreCaseComparison = Argument(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("StringComparison"),
                    IdentifierName("OrdinalIgnoreCase")
                )
            );
            return convertedExpression.AddArgumentListArguments(ignoreCaseComparison);
        }

        private static MemberAccessExpressionSyntax CreateAssertMemberAccessExpressionWithGenericType(string replacement, TypeSyntax type) =>
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("Assert"),
                GenericName(
                    Identifier(replacement),
                    TypeArgumentList(SingletonSeparatedList(type))
                )
            );

        private static InvocationExpressionSyntax ReplaceXUnitAssertionsWithExpectedArgument(
            string expressionName,
            MemberAccessExpressionSyntax memberAccessExpression,
            ArgumentSyntax expectedArgument,
            ArgumentSyntax actualArgument)
        {
            if (ThatAssertionsCompareArgument.TryGetValue(expressionName, out string compareReplacement))
            {
                return CreateXUnitComparsion(compareReplacement, memberAccessExpression, expectedArgument, actualArgument);
            }

            if (ThatAssertionsContainsArgument.TryGetValue(expressionName, out string containsReplacement))
            {
                var assert = CreateAssertMemberAccessExpression(containsReplacement);
                return InvocationExpression(assert, ArgumentList(SeparatedList(new[] { expectedArgument, actualArgument })));
            }

            if (ThatAssertionsWithCompareOperator.ContainsKey(expressionName))
            {
                return CreateCompareOperatorAssert(actualArgument, expectedArgument, ThatAssertionsWithCompareOperator[expressionName]);
            }

            return null;
        }

        private static InvocationExpressionSyntax CreateXUnitComparsion(
            string compareReplacement,
            MemberAccessExpressionSyntax memberAccessExpression,
            ArgumentSyntax expectedArgument,
            ArgumentSyntax actualArgument)
        {
            var assertExpression = CreateAssertMemberAccessExpression(compareReplacement);
            if (memberAccessExpression.IsCollectionSizeExpression())
            {
                return CreateXUnitComparsionForCollection(assertExpression, expectedArgument, actualArgument);
            }
            return InvocationExpression(assertExpression, ArgumentList(SeparatedList(new[] { expectedArgument, actualArgument })));
        }

        private static MemberAccessExpressionSyntax CreateAssertMemberAccessExpression(string replacement) =>
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("Assert"),
                IdentifierName(replacement)
            );

        private static InvocationExpressionSyntax ReplaceCompareOperationAssert(InvocationExpressionSyntax node)
        {
            var expressionName = (node.Expression as MemberAccessExpressionSyntax)?.Name.ToString();
            if (!IsCompareOperatorAssert(node, expressionName))
            {
                return node;
            }

            var left = node.ArgumentList.Arguments[0];
            var right = node.ArgumentList.Arguments[1];
            return CreateCompareOperatorAssert(left, right, CompareOperatorAssertions[expressionName]);
        }

        private static bool IsCompareOperatorAssert(InvocationExpressionSyntax node, string expressionName)
        {
            return !string.IsNullOrEmpty(expressionName) && (CompareOperatorAssertions.ContainsKey(expressionName) && node.ArgumentList.Arguments.Count == 2);
        }

        private static bool IsAssertWithTwoArguments(InvocationExpressionSyntax node)
        {
            // Right now we do for some subset of That assertions, this just skipping if we can not handled.
            // And still keep the old code to let developer convert manually.
            return node.IsAssertThatExpression() && node.ArgumentList.Arguments.Count == 2;
        }

        private static InvocationExpressionSyntax CreateCompareOperatorAssert(
            ArgumentSyntax left,
            ArgumentSyntax right,
            SyntaxKind syntax)
        {
            var assertExpression = CreateAssertMemberAccessExpression("True");
            var argumentList = ArgumentList(SeparatedList(new[]
            {
                Argument(BinaryExpression(syntax, left.Expression, right.Expression))
            })).NormalizeWhitespace();

            return InvocationExpression(assertExpression, argumentList);
        }

        private static InvocationExpressionSyntax CreateXUnitComparsionForCollection(
            MemberAccessExpressionSyntax assertExpression,
            ArgumentSyntax expectedArgument,
            ArgumentSyntax actualArgument)
        {
            var expression = ParseExpression($"{actualArgument}.Count()");
            var newArgument = Argument(expression);
            return InvocationExpression(assertExpression, ArgumentList(SeparatedList(new[] { expectedArgument, newArgument })));
        }

        public static ArgumentSyntax GetExpectedArgument(MemberAccessExpressionSyntax memberAccessExpression) =>
            ((InvocationExpressionSyntax)memberAccessExpression.Parent).ArgumentList.Arguments.FirstOrDefault();

        public static TypeSyntax GetExpectedType(MemberAccessExpressionSyntax memberAccessExpression)
        {
            var genericSyntax = memberAccessExpression.Name as GenericNameSyntax;
            return genericSyntax.TypeArgumentList.Arguments.First();
        }

        public static string GetNUnitExpressionName(MemberAccessExpressionSyntax memberAccessExpression)
        {
            if (memberAccessExpression.IsNotExpression())
            {
                // method argument is something like Is.Not.FooBar, we want to return "Not.FooBar"
                return $"Not.{GetSyntaxName(memberAccessExpression)}";
            }
            if (memberAccessExpression.IsNoExpression())
            {
                // method argument is something like Is.No.FooBar, we want to return "No.FooBar"
                return $"No.{GetSyntaxName(memberAccessExpression)}";
            }
            // method argument is something like Is.FooBar, we want to return "FooBar"
            return GetSyntaxName(memberAccessExpression);
        }


        private static string GetSyntaxName(MemberAccessExpressionSyntax member)
        {
            switch (member.Name)
            {
                case GenericNameSyntax genericName:
                    return genericName.Identifier.Text;
                default:
                    return member.Name.ToString();
            }
        }

        // https://xunit.github.io/docs/comparisons.html
        private static readonly IReadOnlyDictionary<string, string> ThatAssertionsSingleArgument = new Dictionary<string, string>
        {
            ["Is.Null"] = "Null",
            ["Is.True"] = "True",
            ["Is.False"] = "False",
            ["Is.Empty"] = "Empty",
            ["Is.Not.Null"] = "NotNull",
            ["Is.Not.True"] = "False",
            ["Is.Not.False"] = "True",
            ["Is.Not.Empty"] = "NotEmpty",
        };

        private static readonly IReadOnlyDictionary<string, string> ThatAssertionsCompareArgument = new Dictionary<string, string>
        {
            ["EqualTo"] = "Equal",
            ["Not.EqualTo"] = "NotEqual",
            ["SameAs"] = "Same",
            ["Not.SameAs"] = "NotSame"
        };

        private static readonly IReadOnlyDictionary<string, string> ThatAssertionsContainsArgument = new Dictionary<string, string>
        {
            ["StringContaining"] = "Contains",
            ["Not.StringContaining"] = "DoesNotContain",
            ["StringStarting"] = "StartsWith",
            ["EndsWith"] = "EndsWith",
            ["Member"] = "Contains",
            ["No.Member"] = "DoesNotContain",
        };

        private static readonly IReadOnlyDictionary<string, string> ThatAssertionGenericType = new Dictionary<string, string>
        {
            ["InstanceOf"] = "IsType",
            ["Not.InstanceOf"] = "IsNotType"
        };

        private static readonly IReadOnlyDictionary<string, SyntaxKind> CompareOperatorAssertions = new Dictionary<string, SyntaxKind>
        {
            ["Greater"] = SyntaxKind.GreaterThanExpression,
            ["GreaterOrEqual"] = SyntaxKind.GreaterThanOrEqualExpression,
            ["Less"] = SyntaxKind.LessThanExpression,
            ["LessOrEqual"] = SyntaxKind.LessThanOrEqualExpression,
        };

        private static readonly IReadOnlyDictionary<string, SyntaxKind> ThatAssertionsWithCompareOperator = new Dictionary<string, SyntaxKind>
        {
            ["GreaterThan"] = SyntaxKind.GreaterThanExpression,
            ["GreaterThanOrEqualTo"] = SyntaxKind.GreaterThanOrEqualExpression,
            ["LessThan"] = SyntaxKind.LessThanExpression,
            ["LessThanOrEqualTo"] = SyntaxKind.LessThanOrEqualExpression,
        };
    }
}
