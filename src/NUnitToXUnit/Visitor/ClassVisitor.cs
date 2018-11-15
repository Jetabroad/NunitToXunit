// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Features;
using NUnitToXUnit.Features.TestCaseSourceToMemberData;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var withConstructor = SetUpMethodToConstructor.Convert(node);
            var withDisposable = TearDownMethodToDisposable.Convert(withConstructor, requires);
            var withConvertedMemberData = MembersToPublicStaticEnumerable.Convert(withDisposable, requires);
            return base.VisitClassDeclaration(withConvertedMemberData);
        }
    }
}
