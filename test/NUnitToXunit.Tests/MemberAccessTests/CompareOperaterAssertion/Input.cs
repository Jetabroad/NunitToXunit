// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using NUnit.Framework;

namespace NUnitToXUnit.Testing
{
    [TestFixture]
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            Assert.Greater(11, 10);
            Assert.GreaterOrEqual(20, 10);
            Assert.Less(7, 10);
            Assert.LessOrEqual(10, 10);
        }
    }
}
