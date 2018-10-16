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
            Assert.That(actual, expression: Is.EqualTo(expected));
            Assert.That(actual, expression: Is.Not.EqualTo(8));

            var x = false;
            Assert.That(x, expression: Is.False);
            Assert.That(x, expression: Is.Not.True);

            var y = true;
            Assert.That(y, expression: Is.True);
            Assert.That(y, expression: Is.Not.False);

            var w = new int[] { };
            Assert.That(w, expression: Is.Empty);

            var z = new[] { 1, 2, 3 };
            Assert.That(z, expression: Is.Not.Empty);
            Assert.That(z, expression: Is.Not.Null);

            Assert.That(actual, Is.SameAs(foo));
            Assert.That(actual, Is.Not.SameAs(bar));
        }
    }
}
