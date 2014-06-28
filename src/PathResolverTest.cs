namespace PathResolver
{
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

        public string ResolvingDirectoryPathShouldNotChangeCase()
        {
            return null;
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