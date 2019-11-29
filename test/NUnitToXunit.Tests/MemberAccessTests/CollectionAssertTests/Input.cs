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
            var equalArray = new[] { "A", "B" };
            CollectionAssert.AreEqual(new[] { "A", "B" }, equalArray);

            var emptyList = new List<string>();
            CollectionAssert.IsEmpty(emptyList);
        }
    }
}
