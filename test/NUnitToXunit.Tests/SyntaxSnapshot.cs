// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnitToXUnit;
using NUnitToXUnit.Visitor;
using Xunit;

namespace NUnitToXunit.Tests
{
    /// <summary>
    /// Allows for "Snapshot testing" where we have the input and expected output as C# files on disk.
    /// The files' properties should be set as Build Action: "Content"; Copy:"Copy If Newer" to avoid
    /// attempted compilation of these files.
    /// </summary>
    public static class SyntaxSnapshot
    {
        internal static void RunSnapshotTest(string testCategory, string testCase)
        {
            // read input/expected syntax nodes
            var input = ReadCSharpFile(Path.Combine(testCategory, testCase, "Input.cs"));
            var expectedTransformation = ReadCSharpFile(Path.Combine(testCategory, testCase, "Expected.cs"));

            // system under test
            var actualTransformation = new NUnitToXUnitVisitor(new DefaultOption { ConvertAssert = true }).Visit(input);

            var expected = expectedTransformation.ToFullString();
            var actual = actualTransformation.ToFullString();

            Assert.Equal(expected, actual);
        }

        private static SyntaxNode ReadCSharpFile(string filename)
        {
            var source = File.ReadAllText(filename);
            return CSharpSyntaxTree.ParseText(source).GetRoot();
        }
    }
}
