namespace PathResolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PathResolver
    {
        public string ResolvePath(string basePath, string relativePath)
        {
            var relativePathParts = SplitRelativePath(relativePath);

            var pathInfo = this.SplitUpBasePath(basePath);
            var root = pathInfo.Item1;
            var seperator = pathInfo.Item2;
            var basePathParts = pathInfo.Item3;

            for (var i = 0; i < relativePathParts.Count; i++)
            {
                var relativePathPart = relativePathParts[i];
                if (relativePathPart == "..")
                {
                    basePathParts.Remove(basePathParts[basePathParts.Count - 1]);
                }
                else
                {
                    basePathParts.Add(relativePathPart);
                }
            }

            var resolvedPath = root + string.Join(seperator, basePathParts);
            return resolvedPath;
        }

        private static string GetRootFromPath(string path)
        {
            if (path.Contains(":\\"))
            {
            }
            else if (path.StartsWith("\\\\"))
            {
            }

            return string.Empty;
        }

        private static List<string> SplitRelativePath(string relativePath)
        {
            var parts =
                relativePath.Split(@"\/".ToCharArray()).Where(part => !string.IsNullOrWhiteSpace(part.Trim())).ToList();
            return parts;
        }

        private Tuple<string, string, List<string>> SplitUpBasePath(string basePath)
        {
            var root = GetRootFromPath(basePath);
            var pathSeperator = basePath.Contains("\\") ? "\\" : "/";
            basePath = basePath.TrimStart(root.ToCharArray());
            var pathSections = SplitRelativePath(basePath);

            return new Tuple<string, string, List<string>>(root, pathSeperator, pathSections);
        }
    }
}