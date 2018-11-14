// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        private readonly Options options;
        private readonly RequiresImports requires;

        public NUnitToXUnitVisitor(Options options)
        {
            this.options = options;
            this.requires = new RequiresImports();
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var xunitTree = (CompilationUnitSyntax)base.VisitCompilationUnit(node);

            // Remember leading trivia (e.g. license header comments) so we can restore
            // it later. For some reason, manipulating the usings can remove it.
            var comment = xunitTree.GetLeadingTrivia();
            var treeWithTriviaTrimmed = options.ConvertAssert
                ? xunitTree.RemoveNUnitUsing().WithoutLeadingTrivia()
                : xunitTree.WithoutLeadingTrivia();

            // add any usings that were required when visiting the tree
            var additionalUsings = GenerateAdditionalUsings(node.Usings).ToArray();
            var treeWithUsings = AddUsingsToCompilationUnit(treeWithTriviaTrimmed, additionalUsings);

            // restore the leading trivia to the new syntax tree. 
            var treeWithTriviaRestored = treeWithUsings.WithLeadingTrivia(comment);

            return treeWithTriviaRestored;
        }

        /// <summary>
        /// Adds the usings to the compilation unit, adding a blank line after the last using
        /// </summary>
        private static CompilationUnitSyntax AddUsingsToCompilationUnit(CompilationUnitSyntax xunitTree, UsingDirectiveSyntax[] additionalUsings)
        {
            if (additionalUsings.Length == 0)
                return xunitTree;

            // ensure the last using has a blank line after it.
            var lastUsing = additionalUsings.Length - 1;
            additionalUsings[lastUsing] = additionalUsings[lastUsing].WithTrailingTrivia(Whitespace(Environment.NewLine + Environment.NewLine));
            var existingUsings = xunitTree.Usings.ToArray();

            // remove the existing usings, trim the leading trivia, then add all usings back in with the trailing space.
            // this handles the cases of:
            //   - no existing usings
            //   - one existing using that will be replaced
            //   - one existing using that will be appended to
            return xunitTree
                .RemoveNodes(existingUsings, SyntaxRemoveOptions.KeepNoTrivia)
                .WithoutLeadingTrivia()
                .WithUsings(List(existingUsings.Concat(additionalUsings).ToArray()));
        }

        private IEnumerable<UsingDirectiveSyntax> GenerateAdditionalUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            if (requires.System && !HasUsing("System"))
            {
                yield return CreateUsing("System");
            }
            if (requires.SystemCollectionsGeneric && !HasUsing("System.Collections.Generic"))
            {
                yield return CreateUsing("System.Collections.Generic");
            }
            if (requires.XUnit && options.ConvertAssert && !HasUsing("Xunit"))
            {
                yield return CreateUsing("Xunit");
            }

            bool HasUsing(string identifier) =>
                usings.Any(u => u.Name.ToString() == identifier);

            UsingDirectiveSyntax CreateUsing(string identifier) =>
                UsingDirective(IdentifierName(identifier))
                    .NormalizeWhitespace()
                    .WithTrailingTrivia(Whitespace(Environment.NewLine));
        }
    }
}
