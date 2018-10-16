// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class ExpectedExceptionTests
    {
        [Theory]
        [InlineData(nameof(ExpectedExceptionTests), "ToAssertThrowsString")]
        [InlineData(nameof(ExpectedExceptionTests), "ToAssertThrowsStringList")]
        [InlineData(nameof(ExpectedExceptionTests), "ToAssertThrowsTypeOf")]
        [InlineData(nameof(ExpectedExceptionTests), "ToAssertThrowsTypeOfList")]
        public void MethodVisitor_FromExpectedException_ToAssertThrows(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
