// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features
{
    public class TearDownMethodToDisposable
    {
        public static ClassDeclarationSyntax Convert(ClassDeclarationSyntax node, RequiresImports requires)
        {
            var tearDownMethod = node.FindMethodWithAttribute("TearDown").SingleOrDefault();

            // this class has no teardown method
            if (tearDownMethod == null)
            {
                return node;
            }

            requires.System = true;

            var disposeMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Dispose"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithTriviaFrom(tearDownMethod)
                .WithBody(tearDownMethod.Body);

            return node.ReplaceNode(tearDownMethod, disposeMethod)
                       .AddBaseListTypes(SimpleBaseType(IdentifierName("IDisposable")));
        }
    }
}
