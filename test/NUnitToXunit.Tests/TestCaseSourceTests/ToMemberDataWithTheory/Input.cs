// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using NUnit.Framework;

namespace NUnitToXUnit.Testing
{
    /// <summary>
    /// Unit test input to verify [TestCaseSource] to [MemberData] and [Theory]
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

        [TestCaseSource(nameof(partners))]
        public void TestIfTheyLikeEachOther(string a, string b)
        {
        }
    }
}
