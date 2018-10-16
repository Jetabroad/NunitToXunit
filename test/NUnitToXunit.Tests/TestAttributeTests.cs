// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class TestAttributeTests
    {
        [Theory]
        [InlineData(nameof(TestAttributeTests), "ToFact")]
        [InlineData(nameof(TestAttributeTests), "ToTheory")]
        public void AttributeVisitor_FromTest_ToFact(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
