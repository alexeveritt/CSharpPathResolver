namespace PathResolver
{
    using NUnit.Framework;

    [TestFixture]
    public class PathResolverTest
    {
        [TestCase(@"C:\", @"path1", Result = @"C:\path1")]
        [TestCase(@"c:\path1\path2", @"path3", Result = @"")]
        [TestCase(@"c:\path1\path2", @"..", Result = @"..")]
        [TestCase(@"c:\path1\path2", @"../path3", Result = @"")]
        [TestCase(@"c:\path1\path2", @"../../", Result = @"")]
        [TestCase(@"c:\path1\path2", @"../../path3", Result = @"")]
        [TestCase(@"c:\path1\path2", @"../../..", Result = @"")]
        [TestCase(@"\\root\", @"..", Result = @"error")]
        [TestCase(@"//", @"", Result = @"")]
        public string ResolvePathTest(string basePath, string relativePath)
        {
            var resolver = new PathResolver();
            var resolvedPath = resolver.ResolvePath(basePath, relativePath);
            return resolvedPath;
        }
    }
}