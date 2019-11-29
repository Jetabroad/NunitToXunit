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
            var expected = 7;
            var actual = 2 + 5;

            Assert.Null(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(expected, actual);
            Assert.NotEqual(8, actual);
        }
    }
}
