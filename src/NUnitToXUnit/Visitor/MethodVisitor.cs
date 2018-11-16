// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Features.ConvertAttributes;
using NUnitToXUnit.Features.TestCaseSourceToMemberData;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var withAssertThrows = ExpectedExceptionToAssertThrows.Convert(node);
            var visited = (MethodDeclarationSyntax)base.VisitMethodDeclaration(withAssertThrows);
            var result = AddTheoryIfMemberDataPresent.Convert(visited);
            return result;
        }
    }
}
