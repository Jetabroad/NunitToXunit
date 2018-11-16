// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Features.ConvertAttributes;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            var convertedAttributes = ConvertAttributes.Convert(node);
            return base.VisitAttribute(convertedAttributes);
        }

        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            var withoutTestFixture = ConvertTestFixture.Convert(node, requires);
            return base.VisitAttributeList(withoutTestFixture);
        }
    }
}
