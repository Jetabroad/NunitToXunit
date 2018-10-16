// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit.Testing
{
    public class UnitTest
    {
        [Xunit.InlineData("key", "value")]
        public void UnitTest(string key, string value)
        {
        }
    }
}
