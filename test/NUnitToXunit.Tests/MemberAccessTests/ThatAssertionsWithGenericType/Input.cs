// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            Assert.That(result, Is.InstanceOf<FooInstance>());
            Assert.That(result, Is.Not.InstanceOf<FooInstance>());
        }
    }
}
