// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using Xunit;

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Xunit.Fact]
        public void AssertionTest()
        {
            var equalArray = new[] { "A", "B" };
            Assert.Equal(new[] { "A", "B" }, equalArray);

            var emptyList = new List<string>();
            Assert.Empty(emptyList);
        }
    }
}
