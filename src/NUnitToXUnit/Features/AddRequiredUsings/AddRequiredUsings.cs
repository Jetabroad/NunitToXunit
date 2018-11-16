// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using NUnitToXUnit.Extensions;

namespace NUnitToXUnit.Features.AddRequiredUsings
{
    static class AddRequiredUsings
    {
        public static RequiredUsings GetDefaultState() =>
            new RequiredUsings();

        public static CompilationUnitSyntax AddUsings(CompilationUnitSyntax node, RequiredUsings requiredUsings, Options options)
        {
            // Remember leading trivia (e.g. license header comments) so we can restore
            // it later. For some reason, manipulating the usings can remove it.
            var comment = node.GetLeadingTrivia();
            var treeWithTriviaTrimmed = options.ConvertAssert
                ? node.RemoveNUnitUsing().WithoutLeadingTrivia()
                : node.WithoutLeadingTrivia();

            // add any usings that were required when visiting the tree
            var additionalUsings = GenerateAdditionalUsings(node.Usings, requiredUsings, options).ToArray();
            var treeWithUsings = AddUsingsToCompilationUnit(treeWithTriviaTrimmed, additionalUsings);

            // restore the leading trivia to the new syntax tree. 
            var treeWithTriviaRestored = treeWithUsings.WithLeadingTrivia(comment);

            return treeWithTriviaRestored;
        }

        private static IEnumerable<UsingDirectiveSyntax> GenerateAdditionalUsings(SyntaxList<UsingDirectiveSyntax> existing, RequiredUsings requires, Options options)
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
                existing.Any(u => u.Name.ToString() == identifier);

            UsingDirectiveSyntax CreateUsing(string identifier) =>
                UsingDirective(IdentifierName(identifier))
                    .NormalizeWhitespace()
                    .WithTrailingTrivia(Whitespace(Environment.NewLine));
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
    }
}
