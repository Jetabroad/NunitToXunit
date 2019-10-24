// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Features.ConvertAsserts;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (!options.ConvertAssert)
            {
                return node;
            }

            var converted = node
                .ConvertThatExpression()
                .ConvertCollectionAssert();
            
            requires.XUnit = true;

            return base.VisitInvocationExpression(converted);
        }
    }
}
