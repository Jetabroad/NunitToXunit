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

            Assert.That(actual, expression: Is.Null, $"{actual} is not null");
            Assert.That(actual, expression: Is.Not.Empty, "this result should not null");
            Assert.That(actual, expression: Is.EqualTo(expected), $"expected is: {expected} and actual is : {actual}");
            Assert.That(actual, expression: Is.Not.EqualTo(8), "some error message");
        }
    }
}
