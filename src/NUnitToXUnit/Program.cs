// Copyright (c) 2018 Jetabroad Pty Limited. All Rights Reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for license information.

using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnitToXUnit.Visitor;

namespace NUnitToXUnit
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Give preferences to use the path passed as command line argument
            var pathToLookup = args.FirstOrDefault() ?? "{YOUR_TEST_PROJECT_DIRECTORY}";
            const bool convertAssert = false;

            var files = Directory.GetFiles(pathToLookup, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var nUnitTree = CSharpSyntaxTree.ParseText(File.ReadAllText(file)).GetRoot();
                var xUnitTree = new NUnitToXUnitVisitor(new DefaultOption { ConvertAssert = convertAssert }).Visit(nUnitTree);
                File.WriteAllText(file, xUnitTree.ToFullString(), Encoding.UTF8);
            }
        }
    }
}
