// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using NUnit.Framework;
using System.Linq;

namespace NUnitToXUnit.Testing
{
    [TestFixture]
    public class Unittest
    {
        public void SomeMethod()
        {
            var a = Enumerable
                .Range(1, 100)
                .Select(_ => "oh yeah!")
                .ToList();
        }
    }
}
