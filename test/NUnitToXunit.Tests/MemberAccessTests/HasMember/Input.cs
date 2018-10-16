// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            var myNumbers = new[] { 7, 8, 9 };
            var item = 10;
            Assert.That(myNumbers, Has.Member(item));
            Assert.That(myNumbers, Has.No.Member(item));
        }
    }
}
