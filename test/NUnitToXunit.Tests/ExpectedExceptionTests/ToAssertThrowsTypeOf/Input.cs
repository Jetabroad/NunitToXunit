﻿// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class UnitTest
    {
        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestDivisionByZero()
        {
            var a = 1 / 0;
        }
    }
}
