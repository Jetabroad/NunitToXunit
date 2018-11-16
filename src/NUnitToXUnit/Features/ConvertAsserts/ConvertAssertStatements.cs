// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features.ConvertAsserts
{
    static class ConvertAssertStatements
    {
        public static MemberAccessExpressionSyntax Convert(MemberAccessExpressionSyntax node)
        {
            return node.IsAssertExpression() && Assertions.ContainsKey(node.Name.ToString())
                ? CreateAssertExpression(node)
                : node;
        }

        private static MemberAccessExpressionSyntax CreateAssertExpression(MemberAccessExpressionSyntax node)
        {
            return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("Assert").WithTriviaFrom(node),
                    IdentifierName(Assertions[node.Name.ToString()]));
        }

        private static readonly IReadOnlyDictionary<string, string> Assertions = new Dictionary<string, string>
        {
            ["AreEqual"] = "Equal",
            ["AreNotEqual"] = "NotEqual",
            ["AreNotSame"] = "NotSame",
            ["AreSame"] = "Same",
            ["IsEmpty"] = "Empty",
            ["IsFalse"] = "False",
            ["IsInstanceOf"] = "IsType",
            ["IsNotEmpty"] = "NotEmpty",
            ["IsNotInstanceOf"] = "IsNotType",
            ["IsNotNull"] = "NotNull",
            ["IsNull"] = "Null",
            ["IsTrue"] = "True"
        };
    }
}
