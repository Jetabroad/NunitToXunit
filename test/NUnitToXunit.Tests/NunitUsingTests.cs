// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class NunitUsingTests
    {
        [Theory]
        [InlineData(nameof(NunitUsingTests), "KeepsOtherUsings")]
        [InlineData(nameof(NunitUsingTests), "ToXUnitUsing")]
        public void CompilationUnitVisitor_FromNUnitUsing_ToXUnitUsing(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
