// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NUnitToXUnit.Extensions
{
    public static class ExpressionSyntaxExtensions
    {
        public static bool IsAssertExpression(this MemberAccessExpressionSyntax node) =>
            node.Expression.ToString() == "Assert";

        public static bool IsAssertThatExpression(this InvocationExpressionSyntax node) =>
            node.Expression.ToString() == "Assert.That";

        public static bool IsFailExpression(this InvocationExpressionSyntax node) =>
            node.Expression.ToString() == "Assert.Fail";

        public static bool IsCollectionSizeExpression(this MemberAccessExpressionSyntax node) =>
            node.ToString() == "Has.Length.EqualTo" || node.ToString() == "Has.Count.EqualTo";

        public static bool IsNotExpression(this MemberAccessExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax innerMemberAccessExpressionSyntax
                && innerMemberAccessExpressionSyntax.Name.ToString() == "Not";

        public static bool IsNoExpression(this MemberAccessExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax innerExpression
                && innerExpression.Name.ToString() == "No";

    }
}
