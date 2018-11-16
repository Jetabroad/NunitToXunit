// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Features.AddRequiredUsings;

namespace NUnitToXUnit.Visitor
{
    public partial class NUnitToXUnitVisitor : CSharpSyntaxRewriter
    {
        private readonly Options options;
        private readonly RequiredUsings requires;

        public NUnitToXUnitVisitor(Options options)
        {
            this.options = options;
            this.requires = AddRequiredUsings.GetDefaultState();
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var xunitTree = (CompilationUnitSyntax)base.VisitCompilationUnit(node);
            var withUsings = AddRequiredUsings.AddUsings(xunitTree, requires, options);
            return withUsings;
        }
    }
}
