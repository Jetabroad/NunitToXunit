// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features.TestCaseSourceToMemberData
{
    class MembersToPublicStaticEnumerable
    {
        private static readonly SyntaxTrivia Space = Whitespace(" ");
        private static readonly SyntaxTokenList PublicStatic =
            TokenList(
                Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(Space),
                Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(Space)
            );
        private static readonly SyntaxTokenList PublicStaticReadonly =
            PublicStatic.Add(
                Token(SyntaxKind.ReadOnlyKeyword).WithTrailingTrivia(Space)
            );

        public static ClassDeclarationSyntax Convert(ClassDeclarationSyntax node, RequiredUsings requires)
        {
            // get all the TestCaseSource attributes
            var testCaseSources = FindTestCaseSourceAttributes(node);

            if (!testCaseSources.Any())
            {
                return node;
            }

            requires.SystemCollectionsGeneric = true;

            var memberNames = GetMemberNamesFromArguments(testCaseSources);

            var publicStaticFields = GenerateReplacementMembers(node, memberNames);

            // update the syntax tree with the new members
            var rewritten = node.ReplaceNodes(
                publicStaticFields.Keys,
                (oldNode, _) => publicStaticFields[oldNode].WithTriviaFrom(oldNode)
            );

            return rewritten;
        }


        private static List<AttributeSyntax> FindTestCaseSourceAttributes(ClassDeclarationSyntax node)
        {
            return node.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(attr => attr.Name.ToString() == "TestCaseSource")
                .ToList();
        }

        private static List<string> GetMemberNamesFromArguments(List<AttributeSyntax> testCaseSources)
        {
            return testCaseSources
                .Select(tcs =>
                    tcs.ArgumentList.Arguments.Count == 1
                    ? tcs.ArgumentList.Arguments.Single()
                    : throw new NotSupportedException("MemberData on external type not yet supported.")
                )
                .Select(argument =>
                    argument.Expression is InvocationExpressionSyntax invocation && invocation.Expression.ToString() == "nameof"
                    ? invocation.ArgumentList.Arguments.First().ToString() // nameof(Member)
                    : argument.Expression.ToString().Trim('"') // literal string "Member"
                )
                .Distinct()
                .ToList();
        }

        private static Dictionary<MemberDeclarationSyntax, MemberDeclarationSyntax> GenerateReplacementMembers(ClassDeclarationSyntax node, List<string> memberNames)
        {
            // find all the fields that are used as member data
            // a bit awkward that fields and methods are similar, but have no common base class properties / methods
            var fields =
                from field in node.DescendantNodes().OfType<FieldDeclarationSyntax>()
                from variable in field.Declaration.Variables
                where memberNames.Contains(variable.Identifier.ToString())
                select field;
            var methods =
                from method in node.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where memberNames.Contains(method.Identifier.ToString())
                select method;

            // build up our replacement table. all replacements have to happen at once,
            // because otherwise we'd get a new syntax tree on our first replacement,
            // invalidating the later replacements.
            return
                fields.ToDictionary(
                    field => field as MemberDeclarationSyntax,
                    field => field
                                .WithModifiers(PublicStaticReadonly)
                                .WithDeclaration(field.Declaration.WithType(WrapInEnumerable(field.Declaration.Type)))
                                 as MemberDeclarationSyntax
                )
                .Concat(
                    methods.ToDictionary(
                        method => method as MemberDeclarationSyntax,
                        method => method
                                    .WithModifiers(PublicStatic)
                                    .WithReturnType(WrapInEnumerable(method.ReturnType))
                                    as MemberDeclarationSyntax
                    )
                )
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static TypeSyntax WrapInEnumerable(TypeSyntax type)
        {
            // very rough heuristic, if we have something like object[] make it IEnumerable<object[]>
            // as required by xunit. But if the data type is already enumerable, like object[][]
            // or IList<IList<object>> don't touch it
            if (type.ToString() == "IEnumerable<object[]>"
                || type.ToString().EndsWith("[][]")
                || type.ToString().Count(c => c == '<') >= 2)
            {
                return type.WithoutLeadingTrivia();
            }
            return ParseTypeName($"IEnumerable<{type}>").WithTrailingTrivia(Space);
        }
    }
}
