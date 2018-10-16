// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

namespace NUnitToXUnit
{
    public interface IConverterOptions
    {
        bool RequiresSystemImport { get; set; }
        bool RequiresXUnitImport { get; set; }
        bool ConvertAssert { get; set; }
    }

    public class DefaultOption : IConverterOptions
    {
        public bool RequiresSystemImport { get; set; }
        public bool RequiresXUnitImport { get; set; }
        public bool ConvertAssert { get; set; }
    }
}
