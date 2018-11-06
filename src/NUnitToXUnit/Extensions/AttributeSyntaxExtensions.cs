// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace NUnitToXUnit.Extensions
{
    public static class AttributeSyntaxExtensions
    {
        public static string GetExceptionType(this AttributeArgumentSyntax node) =>
            node.Expression.ChildNodes().First().ToString();

        public static AttributeArgumentSyntax GetExpectedExceptionAttributeArgument(this AttributeSyntax node) =>
            node.ArgumentList?.Arguments.FirstOrDefault();

        public static bool IsExceptionMessage(this AttributeArgumentSyntax node) =>
            node.Expression is LiteralExpressionSyntax;

        public static bool IsTypeOfException(this AttributeArgumentSyntax node) =>
            node.Expression is TypeOfExpressionSyntax;

        public static bool IsTestCaseSourceAttribute(this AttributeSyntax node) =>
            node.Name.ToString() == "TestCaseSource";

        public static bool IsExpectedExceptionAttribute(this AttributeSyntax node) =>
            node.Name.ToString() == "ExpectedException";

        public static bool IsTestFixtureAttribute(this AttributeSyntax node) =>
            node.Name.ToString() == "TestFixture";

        public static IEnumerable<AttributeArgumentSyntax> GetAttributeArguments(this SeparatedSyntaxList<AttributeSyntax> node) =>
           node.Where(attr => attr.ArgumentList?.Arguments.Any() == true)
               .SelectMany(attr => attr.ArgumentList.Arguments);
    }
}
