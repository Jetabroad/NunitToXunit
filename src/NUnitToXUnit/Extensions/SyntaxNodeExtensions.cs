// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace NUnitToXUnit.Extensions
{
    /// <summary>
    /// General extensions for SyntaxNode and descendants.
    /// </summary>
    public static class SyntaxNodeExtensions
    {
        public static bool IsPrivate(this SyntaxToken node) => node.Text == "private";

        public static string GetNodeIdentifier(this FieldDeclarationSyntax node) =>
            node.Declaration.Variables.First().Identifier.ValueText;

        public static bool IsNUnitUsing(this UsingDirectiveSyntax node) =>
            node.Name.ToString() == "NUnit.Framework";

        public static CompilationUnitSyntax RemoveNUnitUsing(this CompilationUnitSyntax node)
        {
            var nUnitUsing = node.Usings.SingleOrDefault(IsNUnitUsing);
            if (nUnitUsing == null) return node;
            return node.RemoveNode(nUnitUsing, SyntaxRemoveOptions.KeepLeadingTrivia);
        }
    }
}
