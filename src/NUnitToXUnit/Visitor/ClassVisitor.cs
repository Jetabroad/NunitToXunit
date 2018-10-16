// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var withConstructor = ConvertSetUpMethodToConstructor(node);
            var withDisposable = ConvertTearDownMethodToDisposable(withConstructor);
            return base.VisitClassDeclaration(withDisposable);
        }

        private static ClassDeclarationSyntax ConvertSetUpMethodToConstructor(ClassDeclarationSyntax node)
        {
            var setUpMethod = GetMethodWithAttribute(node, "SetUp");
            // this class has no setup method
            if (setUpMethod == null)
            {
                return node;
            }

            // create a constructor
            var constructor = ConstructorDeclaration(Identifier(node.Identifier.Text))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithTriviaFrom(setUpMethod)
                .WithBody(setUpMethod.Body);

            return node.ReplaceNode(setUpMethod, constructor);
        }

        private ClassDeclarationSyntax ConvertTearDownMethodToDisposable(ClassDeclarationSyntax node)
        {
            var tearDownMethod = GetMethodWithAttribute(node, "TearDown");

            // this class has no teardown method
            if (tearDownMethod == null)
            {
                return node;
            }

            _options.RequiresSystemImport = true;

            var disposeMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Dispose"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithTriviaFrom(tearDownMethod)
                .WithBody(tearDownMethod.Body);

            return node.ReplaceNode(tearDownMethod, disposeMethod)
                       .AddBaseListTypes(SimpleBaseType(IdentifierName("IDisposable")));
        }

        private static MethodDeclarationSyntax GetMethodWithAttribute(ClassDeclarationSyntax node, string attributeName)
        {
            return node.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .SingleOrDefault(method => method.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.Name.ToString() == attributeName));
        }
    }
}
