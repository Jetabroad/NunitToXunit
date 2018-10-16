// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXunit.Tests
{
    public class CategoryTests
    {
        [Theory]
        [InlineData(nameof(CategoryTests), "ToTrait")]
        public void AttributeVisitor_FromCategory_ToTrait(string testCategory, string testCase) =>
            SyntaxSnapshot.RunSnapshotTest(testCategory, testCase);
    }
}
