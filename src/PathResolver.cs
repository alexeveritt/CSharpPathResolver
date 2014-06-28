namespace PathResolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PathResolver
    {
        private readonly IDictionary<PathStyle, Func<string, string, string>> pathResolver;

        public PathResolver()
        {
            pathResolver = new Dictionary<PathStyle, Func<string, string, string>>
                               {
                                   { PathStyle.Directory, ResolveDirectoryPath },
                                   { PathStyle.Unc, ResolveUncPath },
                                   { PathStyle.Http, ResolveHttpPath },
                                   { PathStyle.Linux, ResolveLinuxPath }
                               };
        }

        private enum PathStyle
        {
            Directory = 1,
            Unc = 2,
            Http = 3,
            Linux = 4
        }

        public static string ResolveDirectoryPath(string basePath, string relativePath)
        {
            var pos = basePath.IndexOf(@":\", StringComparison.Ordinal) + 2;
            var root = basePath.Substring(0, pos);
            var path = basePath.Substring(pos);

            var newPath = BuildNewPath(path, relativePath);
            return string.Format("{0}{1}", root, string.Join(@"\", newPath));
        }

        public static string ResolveHttpPath(string basePath, string relativePath)
        {
            var posProtocolSep = basePath.IndexOf("//", StringComparison.Ordinal) + 2;
            var pos = basePath.IndexOf("/", posProtocolSep, StringComparison.Ordinal);
            var root = pos > 0 ? basePath.Substring(0, pos) : basePath;
            var path = pos > 0 ? basePath.Substring(pos) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);
            return newPath.Any() ? string.Format("{0}/{1}", root, string.Join("/", newPath)) : root;
        }

        public static string ResolveLinuxPath(string basePath, string relativePath)
        {
            if (!basePath.StartsWith("/"))
            {
                throw new Exception("Invalid Path format. Path must start with a single /");
            }

            const int Pos = 1;
            var path = basePath.Length > 1 ? basePath.Substring(Pos) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);
            return string.Format("/{0}", string.Join("/", newPath));
        }

        public static string ResolveUncPath(string basePath, string relativePath)
        {
            if (!basePath.StartsWith(@"\\"))
            {
                throw new Exception("Invalid Path format. Path must start with \\");
            }

            var pos = basePath.IndexOf(@"\", 2, StringComparison.Ordinal);
            var root = pos > 0 ? basePath.Substring(0, pos) : basePath;
            var path = pos > 0 ? basePath.Substring(pos) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);
            return newPath.Any() ? string.Format(@"{0}\{1}", root, string.Join(@"\", newPath)) : root;
        }

        public string ResolvePath(string basePath, string relativePath)
        {
            return pathResolver[GetPathStyle(basePath)](basePath, relativePath);
        }

        private static IList<string> BuildNewPath(string path, string relativePath)
        {
            var basePathSegments = SplitPath(path);
            var relativePathSegments = SplitPath(relativePath);

            foreach (var relativeSegment in relativePathSegments)
            {
                if (relativeSegment == "..")
                {
                    basePathSegments.Remove(basePathSegments[basePathSegments.Count - 1]);
                }
                else
                {
                    basePathSegments.Add(relativeSegment);
                }
            }

            return basePathSegments;
        }

        private static PathStyle GetPathStyle(string basePath)
        {
            var lookUp = new List<KeyValuePair<Func<string, bool>, PathStyle>>
                             {
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.Contains(@":\"), PathStyle.Directory),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.StartsWith(@"\\"), PathStyle.Unc),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.Contains("//"), PathStyle.Http),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.StartsWith("/"), PathStyle.Linux)
                             };

            var pathStyle = lookUp.Where(x => x.Key(basePath)).Select(x => x.Value).FirstOrDefault();
            if (0 == pathStyle)
            {
                throw new Exception("Path Style not supported");
            }

            return pathStyle;
        }

        private static List<string> SplitPath(string relativePath)
        {
            var parts =
                relativePath.Split(@"\/".ToCharArray()).Where(part => !string.IsNullOrWhiteSpace(part.Trim())).ToList();
            return parts;
        }
    }
}