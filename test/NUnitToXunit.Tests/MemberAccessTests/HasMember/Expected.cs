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
            var myNumbers = new[] { 7, 8, 9 };
            var item = 10;
            Assert.Contains(item, myNumbers);
            Assert.DoesNotContain(item, myNumbers);
        }
    }
}
