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
            var a = 5;
            var b = 2;
            var c = a + b;

            Assert.AreEqual(7, c);
            Assert.AreNotEqual(8, c);

            string d = string.empty;
            Assert.IsEmpty(d);
            Assert.IsNotNull(d);

            string e = null;
            Assert.IsNull(e);
            Assert.IsNotEmpty(e);

            var h = true;
            Assert.IsTrue(h);
            Assert.IsFalse(!h);

            Assert.Fail("Message.");
        }
    }
}
