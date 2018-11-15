// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class TestCaseSourceTests
    {
        [Theory]
        [InlineData(nameof(TestCaseSourceTests), "ToMemberData")]
        [InlineData(nameof(TestCaseSourceTests), "ToMemberDataWithTheory")]
        public void Convert(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
