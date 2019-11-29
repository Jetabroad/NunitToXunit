using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnitToXUnit.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NUnitToXUnit.Features.ConvertAsserts
{
    public static partial class InvocationExpressionConvertExtension
    {
        public static InvocationExpressionSyntax ConvertCollectionAssert(this InvocationExpressionSyntax originalNode)
        {
            if (!originalNode.IsCollectionAssertExpression()) return originalNode;

            var newExpression = MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("Assert"),
                            GetExpressionName(originalNode.Expression as MemberAccessExpressionSyntax));
            return InvocationExpression(newExpression, ArgumentList(originalNode.ArgumentList.Arguments))
                .WithTriviaFrom(originalNode);
        }

        private static IdentifierNameSyntax GetExpressionName(MemberAccessExpressionSyntax expressionSyntax)
        {            
            return IdentifierName(ConvertAssertStatements.Assertions[expressionSyntax.Name.ToString()]);
        }
    }
}
