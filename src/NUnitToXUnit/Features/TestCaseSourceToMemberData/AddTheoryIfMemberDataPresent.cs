// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features.TestCaseSourceToMemberData
{
    public static class AddTheoryIfMemberDataPresent
    {
        public static SyntaxNode Convert(MethodDeclarationSyntax method)
        {
            var methodAttributes = method
                .AttributeLists
                .SelectMany(list => list.Attributes)
                .Select(attr => attr.Name.ToString())
                .ToList();

            if (!IsMissingTheoryAttribute(methodAttributes))
            {
                return method;
            }

            var attributeList = AddTheoryAttributeToMethod(method);

            return method.WithAttributeLists(attributeList);
        }

        private static SyntaxList<AttributeListSyntax> AddTheoryAttributeToMethod(MethodDeclarationSyntax method)
        {
            var newTheory = AttributeList(SingletonSeparatedList(
                Attribute(ParseName("Xunit.Theory"))
            ));

            var firstAttribute = method.AttributeLists.First();
            var updatedList = method.AttributeLists.Replace(
                firstAttribute,
                firstAttribute.WithLeadingTrivia(firstAttribute.GetLeadingTrivia().Where(trivia => trivia.Kind() != SyntaxKind.EndOfLineTrivia))
            );
            var attributeList = updatedList.Insert(0, newTheory.WithTriviaFrom(method.AttributeLists.First()));
            return attributeList;
        }

        private static bool IsMissingTheoryAttribute(IList<string> methodAttributes)
        {
            var memberData = methodAttributes.FirstOrDefault(attr => attr == "Xunit.MemberData");
            var theory = methodAttributes.FirstOrDefault(attr => attr == "Xunit.Theory");
            return memberData != null && theory == null;
        }
    }
}
