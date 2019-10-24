// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class MemberAccessTests
    {
        [Theory]
        [InlineData(nameof(MemberAccessTests), "XUnitAssertion")]
        [InlineData(nameof(MemberAccessTests), "HasMember")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertions")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertionsWithUnhandled")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertionWithStringContaining")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertionsWithGenericType")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertionsCollection")]
        [InlineData(nameof(MemberAccessTests), "CompareOperatorAssertion")]
        [InlineData(nameof(MemberAccessTests), "ThatAssertionsWithCompareOperator")]
        [InlineData(nameof(MemberAccessTests), "CollectionAssertTests")]
        public void MemberAccessVisitor_FromNUnitAssert_ToXUnitAssertion(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
