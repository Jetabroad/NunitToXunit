// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using NUnit.Framework;

namespace NUnitToXUnit.Testing
{
    /// <summary>
    /// Unit test input to verify [TestCaseSource] to [MemberData]
    /// </summary>
    [TestFixture]
    public class Unittest
    {
        // field with private keyword
        private object[] partners =
        {
            new object[] { "pinky", "brain"},
            new object[] { "cony", "brown"},
            new object[] { "rocky", "bullwinkle"}
        };

        // field with default private visibility
        object[] enemies =
        {
            new object[] { "donald", "bugs"},
            new object[] { "marvin", "bugs"},
            new object[] { "everybody", "bugs"}
        };

        // method with default private visibility
        object[] strangers() => new[]
        {
            new object[] { "Doctor Who", "Doctor Horrible"},
            new object[] { "Captain Planet", "Captain Underpants"},
            new object[] { "Sponge Bob", "Sponge Cake"}
        };

        [Test]
        [TestCaseSource(nameof(partners))]
        public void TestIfTheyLikeEachOther(string a, string b)
        {
        }

        [Test]
        [TestCaseSource("enemies")]
        public void TestIfTheyHateEachOther(string a, string b)
        {
        }

        [Test]
        [TestCaseSource(nameof(strangers))]
        public void TestIfTheyKnowEachOther(string a, string b)
        {
        }
    }
}
