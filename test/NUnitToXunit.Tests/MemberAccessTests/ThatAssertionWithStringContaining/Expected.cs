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
            Assert.Contains("World", "Hello World");
            Assert.Contains("WORLD", "Hello World", StringComparison.OrdinalIgnoreCase);

            Assert.DoesNotContain("WORLD", "Hello World");
            Assert.DoesNotContain("FOO", "Hello World", StringComparison.OrdinalIgnoreCase);

            Assert.StartsWith("Hello", "Hello World");
            Assert.StartsWith("HELLO", "Hello World", StringComparison.OrdinalIgnoreCase);

            Assert.EndsWith("World", "Hello World");
            Assert.EndsWith("WORLD", "Hello World", StringComparison.OrdinalIgnoreCase);
        }
    }
}
