//    The MIT License (MIT)
//
//    Copyright (c) 2014 Alex Everitt
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE. 

namespace PathResolver
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PathResolverTest
    {
        [TestCase(@"c:\", @"path1", Result = @"c:\path1")]
        [TestCase(@"c:\path1\path2", @"path3", Result = @"c:\path1\path2\path3")]
        [TestCase(@"c:\path1\path2", @"..", Result = @"c:\path1")]
        [TestCase(@"c:\path1\path2", @"..\..\path3", Result = @"c:\path3")]
        [TestCase(@"\\path1\path2", @"path3", Result = @"\\path1\path2\path3")]
        [TestCase(@"\\path1\path2", @"..", Result = @"\\path1")]
        [TestCase(@"\\path1\path2", @"..\path3", Result = @"\\path1\path3")]
        [TestCase(@"\\path1\", @"path2", Result = @"\\path1\path2")]
        [TestCase(@"//path1/path2", @"path3", Result = @"//path1/path2/path3")]
        [TestCase(@"//path1/path2", @"../path3", Result = @"//path1/path3")]
        [TestCase(@"//path1/path2", @"../path3/path4", Result = @"//path1/path3/path4")]
        [TestCase(@"http://path1/path2", @"path3", Result = @"http://path1/path2/path3")]
        [TestCase(@"http://path1/path2", @"../path3", Result = @"http://path1/path3")]
        [TestCase(@"http://path1/path2", @"../path3/path4", Result = @"http://path1/path3/path4")]
        [TestCase(@"http://path1:23512/path2", @"path3", Result = @"http://path1:23512/path2/path3")]
        [TestCase(@"http://path1:23512/path2", @"../path3", Result = @"http://path1:23512/path3")]
        [TestCase(@"http://path1:23512/path2", @"../path3/path4", Result = @"http://path1:23512/path3/path4")]
        [TestCase(@"/", @"path1", Result = @"/path1")]
        [TestCase(@"/path1/path2", @"path3", Result = @"/path1/path2/path3")]
        [TestCase(@"/path1/path2", @"..", Result = @"/path1")]
        [TestCase(@"/path1/path2", @"../..", Result = @"/")]
        [TestCase(@"/path1/path2", @"../path3", Result = @"/path1/path3")]
        [TestCase(@"/path1/path2", @"../../path3", Result = @"/path3")]
        public string ResolvePathTest(string basePath, string relativePath)
        {
            var resolver = new PathResolver();
            var resolvedPath = resolver.ResolvePath(basePath, relativePath);
            return resolvedPath;
        }

        [TestCase(@"C:\", @"path1", @"C:\path1")]
        [TestCase(@"c:\pATh1\path2", @"patH3", @"c:\pATh1\path2\patH3")]
        [TestCase(@"c:\path1\path2", @"..\..\Path3", @"c:\Path3")]
        [TestCase(@"\\Path1\Path2", @"..", @"\\Path1")]
        [TestCase(@"\\path1\path2", @"..\paTH3", @"\\path1\paTH3")]
        [TestCase(@"//Path1/path2", @"Path3", @"//Path1/path2/Path3")]
        [TestCase(@"//path1/path2", @"../Path3", @"//path1/Path3")]
        [TestCase(@"http://Path1/path2", @"path3", @"http://Path1/path2/path3")]
        [TestCase(@"http://path1/path2", @"../PatH3", @"http://path1/PatH3")]
        [TestCase(@"http://Path1/path2", @"../path3/path4", @"http://Path1/path3/path4")]
        [TestCase(@"http://path1:23512/pATh2", @"path3", @"http://path1:23512/pATh2/path3")]
        [TestCase(@"http://Path1:23512/path2", @"../path3", @"http://Path1:23512/path3")]
        [TestCase(@"http://path1:23512/path2", @"../pAth3/path4", @"http://path1:23512/pAth3/path4")]
        [TestCase(@"/", @"PAth1", @"/PAth1")]
        [TestCase(@"/path1/path2", @"Path3", @"/path1/path2/Path3")]
        public void ResolvingDirectoryPathShouldNotChangeCase(string basePath, string relativePath, string expectedResult)
        {
            var resolver = new PathResolver();
            var result = resolver.ResolvePath(basePath, relativePath);

            Assert.AreEqual(expectedResult, result);
            Assert.AreNotEqual(expectedResult.ToLower(), result);
        }

        [TestCase(@"//path1/path2", @"../../Path3")]
        public void RelativePathThrowsExceptionIfRootPassed(string basePath, string relativePath)
        {
            var resolver = new PathResolver();
            var ex = Assert.Throws<Exception>(() => resolver.ResolvePath(basePath, relativePath));
            Assert.That(ex.Message, Is.EqualTo("Relative path out of bounds"));
        }

        [TestCase(@"c:\", @"path1", Result = @"c:\path1")]
        [TestCase(@"c:\path1\path2", @"path3", Result = @"c:\path1\path2\path3")]
        [TestCase(@"c:\path1\path2", @"..", Result = @"c:\path1")]
        [TestCase(@"c:\path1\path2", @"..\..\path3", Result = @"c:\path3")]
        public string ResolveDirectoryPath(string basePath, string relativePath)
        {
            var result = PathResolver.ResolveDirectoryPath(basePath, relativePath);
            return result;
        }

        [TestCase(@"\\path1\path2", @"path3", Result = @"\\path1\path2\path3")]
        [TestCase(@"\\path1\path2", @"..", Result = @"\\path1")]
        [TestCase(@"\\path1\path2", @"..\path3", Result = @"\\path1\path3")]
        [TestCase(@"\\path1\", @"path2", Result = @"\\path1\path2")]
        public string ResolveUncPath(string basePath, string relativePath)
        {
            var result = PathResolver.ResolveUncPath(basePath, relativePath);
            return result;
        }

        [TestCase(@"//path1/path2", @"path3", Result = @"//path1/path2/path3")]
        [TestCase(@"//path1/path2", @"../path3", Result = @"//path1/path3")]
        [TestCase(@"//path1/path2", @"../path3/path4", Result = @"//path1/path3/path4")]
        [TestCase(@"http://path1/path2", @"path3", Result = @"http://path1/path2/path3")]
        [TestCase(@"http://path1/path2", @"../path3", Result = @"http://path1/path3")]
        [TestCase(@"http://path1/path2", @"../path3/path4", Result = @"http://path1/path3/path4")]
        [TestCase(@"http://path1:23512/path2", @"path3", Result = @"http://path1:23512/path2/path3")]
        [TestCase(@"http://path1:23512/path2", @"../path3", Result = @"http://path1:23512/path3")]
        [TestCase(@"http://path1:23512/path2", @"../path3/path4", Result = @"http://path1:23512/path3/path4")]
        public string ResolveHttpPath(string basePath, string relativePath)
        {
            var result = PathResolver.ResolveUrlPath(basePath, relativePath);
            return result;
        }

        [TestCase(@"/", @"path1", Result = @"/path1")]
        [TestCase(@"/path1/path2", @"path3", Result = @"/path1/path2/path3")]
        [TestCase(@"/path1/path2", @"..", Result = @"/path1")]
        [TestCase(@"/path1/path2", @"../..", Result = @"/")]
        [TestCase(@"/path1/path2", @"../path3", Result = @"/path1/path3")]
        [TestCase(@"/path1/path2", @"../../path3", Result = @"/path3")]
        public string ResolveLinuxPath(string basePath, string relativePath)
        {
            var result = PathResolver.ResolveLinuxPath(basePath, relativePath);
            return result;
        }
    }
}