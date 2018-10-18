// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        private readonly IConverterOptions _options;

        public NUnitToXUnitVisitor(IConverterOptions options)
        {
            _options = options;
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var xunitTree = (CompilationUnitSyntax)base.VisitCompilationUnit(node);

            // For LeadingTrivia can be comments, newline or white space, and some file contain the document,
            // summary or license on top of file as comment, that's need to keep it. 
            var comment = xunitTree.GetLeadingTrivia();

            // remove all leading trivia from syntax tree.
            xunitTree = xunitTree.WithoutLeadingTrivia();
            var additionalUsings = new List<UsingDirectiveSyntax>();

            if (_options.RequiresSystemImport)
            {
                additionalUsings.Add(UsingDirective(IdentifierName("System")));
            }
            if (_options.RequiresXUnitImport && _options.ConvertAssert)
            {
                additionalUsings.Add(UsingDirective(IdentifierName("Xunit")));
            }

            var newTree = xunitTree.AddUsings(additionalUsings.ToArray());

            // restore to comment to the new syntax tree. 
            newTree = newTree.WithLeadingTrivia(comment);

            return _options.ConvertAssert ? newTree.RemoveNUnitUsing() : base.VisitCompilationUnit(newTree);
        }
    }
}
