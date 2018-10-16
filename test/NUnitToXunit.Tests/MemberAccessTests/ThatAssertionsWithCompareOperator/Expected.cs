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
            Assert.True(11 > 10);
            Assert.True(20 >= 10);
            Assert.True(7 < 10);
            Assert.True(10 <= 10);
        }
    }
}
