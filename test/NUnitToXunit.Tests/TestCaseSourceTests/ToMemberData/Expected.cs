// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.Collections.Generic;
using Xunit;

namespace NUnitToXUnit.Testing
{
    /// <summary>
    /// Unit test input to verify [TestCaseSource] to [MemberData]
    /// </summary>
    public class Unittest
    {
        // field with private keyword
        public static readonly IEnumerable<object[]> partners =
        {
            new object[] { "pinky", "brain"},
            new object[] { "cony", "brown"},
            new object[] { "rocky", "bullwinkle"}
        };

        // field with default private visibility
        public static readonly IEnumerable<object[]> enemies =
        {
            new object[] { "donald", "bugs"},
            new object[] { "marvin", "bugs"},
            new object[] { "everybody", "bugs"}
        };

        // method with default private visibility
        public static IEnumerable<object[]> strangers() => new[]
        {
            new object[] { "Doctor Who", "Doctor Horrible"},
            new object[] { "Captain Planet", "Captain Underpants"},
            new object[] { "Sponge Bob", "Sponge Cake"}
        };

        [Xunit.Theory]
        [Xunit.MemberData(nameof(partners))]
        public void TestIfTheyLikeEachOther(string a, string b)
        {
        }

        [Xunit.Theory]
        [Xunit.MemberData("enemies")]
        public void TestIfTheyHateEachOther(string a, string b)
        {
        }

        [Xunit.Theory]
        [Xunit.MemberData(nameof(strangers))]
        public void TestIfTheyKnowEachOther(string a, string b)
        {
        }
    }
}
