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
            var a = 5;
            var b = 2;
            var c = a + b;

            Assert.Equal(7, c);
            Assert.NotEqual(8, c);

            string d = string.empty;
            Assert.Empty(d);
            Assert.NotNull(d);

            string e = null;
            Assert.Null(e);
            Assert.NotEmpty(e);

            var h = true;
            Assert.True(h);
            Assert.False(!h);

            Assert.True(false, "Message.");
        }
    }
}
