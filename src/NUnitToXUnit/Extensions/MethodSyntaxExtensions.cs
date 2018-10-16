// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace NUnitToXUnit.Extensions
{
    public static class MethodSyntaxExtensions
    {
        public static bool HasParameter(this BaseMethodDeclarationSyntax method) =>
            method.ParameterList.Parameters.Any();

        public static bool HasExpectedExceptionAttribute(this MethodDeclarationSyntax node) =>
            node.AttributeLists.SelectMany(list => list.Attributes).Any(attr => attr.IsExpectedExceptionAttribute());

        public static AttributeSyntax GetExpectedExceptionAttribute(this MethodDeclarationSyntax node) =>
            node.AttributeLists.SelectMany(list => list.Attributes).Single(attr => attr.IsExpectedExceptionAttribute());
    }
}
