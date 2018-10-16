// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class Unittest
    {
        [Test]
        public void AssertionTest()
        {
            var list = new List<int>(new[] { 1, 2 });
            Assert.That(list, Has.Length.EqualTo(2));
        }
    }
}
