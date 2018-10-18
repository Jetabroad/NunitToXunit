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
            Assert.That(11, Is.GreaterThan(10));
            Assert.That(20, Is.GreaterThanOrEqualTo(10));
            Assert.That(7, Is.LessThan(10));
            Assert.That(10, Is.LessThanOrEqualTo(10));
        }
    }
}
