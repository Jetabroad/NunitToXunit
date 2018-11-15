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
    public class SetUpMethodToConstructor
    {
        public static ClassDeclarationSyntax Convert(ClassDeclarationSyntax node)
        {
            var setUpMethod = node.FindMethodWithAttribute("SetUp").SingleOrDefault();

            // this class has no setup method
            if (setUpMethod == null)
            {
                return node;
            }

            // create a constructor
            var constructor = ConstructorDeclaration(Identifier(node.Identifier.Text))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword))).NormalizeWhitespace()
                .WithTriviaFrom(setUpMethod)
                .WithBody(setUpMethod.Body);

            return node.ReplaceNode(setUpMethod, constructor);
        }
    }
}
