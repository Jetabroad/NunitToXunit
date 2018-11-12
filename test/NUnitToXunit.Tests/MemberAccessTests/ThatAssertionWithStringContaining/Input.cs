// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            Assert.That("Hello World", Is.StringContaining("World"));
            Assert.That("Hello World", Is.StringContaining("WORLD").IgnoreCase);

            Assert.That("Hello World", Is.Not.StringContaining("WORLD"));
            Assert.That("Hello World", Is.Not.StringContaining("FOO").IgnoreCase);

            Assert.That("Hello World", Is.StringStarting("Hello"));
            Assert.That("Hello World", Is.StringStarting("HELLO").IgnoreCase);

            Assert.That("Hello World", Is.EndsWith("World"));
            Assert.That("Hello World", Is.EndsWith("WORLD").IgnoreCase);
        }
    }
}
