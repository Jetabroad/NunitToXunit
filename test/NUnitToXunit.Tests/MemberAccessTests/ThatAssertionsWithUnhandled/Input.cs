// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            var expected = 7;
            var actual = 2 + 5;

            Assert.That(actual, expression: Is.Null);
            Assert.That(actual, expression: Is.Not.Empty);
            Assert.That(actual, expression: Is.NotEqualTo(expected), message: "Foo");
            Assert.That(actual, expression: Is.EqualTo(expected), message: "Bar");
        }
    }
}
