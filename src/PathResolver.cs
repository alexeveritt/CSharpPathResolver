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
    using System.Collections.Generic;
    using System.Linq;

    public class PathResolver
    {
        #region Exception Messages

        private const string InvalidPathStyleExceptionMessage = "Path Style not supported";
        private const string LinuxErrorExceptionMessage = "Invalid Path format. Path must start with a single /";
        private const string UncErrorExceptionMessage = "Invalid Path format. Path must start with \\";
        private const string RelativePathOutOfBounds = "Relative path out of bounds";

        #endregion Exception Messages

        private readonly IDictionary<PathStyle, Func<string, string, string>> pathResolver;

        public PathResolver()
        {
            // Initialise list of functions used to resolve paths
            pathResolver = new Dictionary<PathStyle, Func<string, string, string>>
                               {
                                   { PathStyle.Directory, ResolveDirectoryPath },
                                   { PathStyle.Unc, ResolveUncPath },
                                   { PathStyle.Url, ResolveUrlPath },
                                   { PathStyle.Linux, ResolveLinuxPath }
                               };
        }

        private enum PathStyle
        {
            Directory = 1,
            Unc = 2,
            Url = 3,
            Linux = 4
        }

        /// <summary>
        /// Resolves a relative path to a windows style directory start with
        /// a letter such as C:\
        /// </summary>
        /// <param name="basePath">Intial source or starting path</param>
        /// <param name="relativePath">Relative path pointing to target</param>
        /// <returns>A full directory path to target based on the resolved relative path</returns>
        public static string ResolveDirectoryPath(string basePath, string relativePath)
        {
            // Get the index position of the first character after the path root
            var pos = basePath.IndexOf(@":\", StringComparison.Ordinal) + 2;

            // Get the root/domain from the path
            var root = basePath.Substring(0, pos);

            // Get the path without the root/domain
            var path = basePath.Substring(pos);

            var newPath = BuildNewPath(path, relativePath);
            return string.Format("{0}{1}", root, string.Join(@"\", newPath));
        }

        /// <summary>
        /// Resolves a relative path to a linux style path, where the seperators
        /// are forward slashes and the root of the path is a forward slash
        /// </summary>
        /// <param name="basePath">Intial source or starting path</param>
        /// <param name="relativePath">Relative path pointing to target</param>
        /// <returns>A full linux style path to target based on the resolved relative path</returns>
        public static string ResolveLinuxPath(string basePath, string relativePath)
        {
            if (!basePath.StartsWith("/"))
            {
                throw new Exception(LinuxErrorExceptionMessage);
            }

            // Check to see if the basepath is more than just /
            var path = basePath.Length > 1 ? basePath.Substring(1) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);

            // Append and return the new path onto the root
            return string.Format("/{0}", string.Join("/", newPath));
        }

        /// <summary>
        /// Resolves a relative path to a unc path
        /// </summary>
        /// <param name="basePath">Intial source or starting path</param>
        /// <param name="relativePath">Relative path pointing to target</param>
        /// <returns>A full unc path to target based on the resolved relative path</returns>
        public static string ResolveUncPath(string basePath, string relativePath)
        {
            if (!basePath.StartsWith(@"\\"))
            {
                throw new Exception(UncErrorExceptionMessage);
            }

            // Find the first slash after the intial \\
            var pos = basePath.IndexOf(@"\", 2, StringComparison.Ordinal);

            // Get the root of the unc path
            var root = pos > 0 ? basePath.Substring(0, pos) : basePath;

            // Get the rest of the path minus the unc root
            var path = pos > 0 ? basePath.Substring(pos) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);

            // Return the new path or just the root if newPath was empty
            return newPath.Any() ? string.Format(@"{0}\{1}", root, string.Join(@"\", newPath)) : root;
        }

        /// <summary>
        /// Resolves a path relative to a url style format such as http://, ftp://
        /// Port numbers are also supported on the domain
        /// </summary>
        /// <param name="basePath">Intial source or starting path</param>
        /// <param name="relativePath">Relative path pointing to target</param>
        /// <returns>A full url to target based on the resolved relative path</returns>
        public static string ResolveUrlPath(string basePath, string relativePath)
        {
            // Get the first position after the double slashes
            var posProtocolSep = basePath.IndexOf("//", StringComparison.Ordinal) + 2;

            // Get the index position of the first character after the domain
            var pos = basePath.IndexOf("/", posProtocolSep, StringComparison.Ordinal);

            // Get the domain from the url
            var root = pos > 0 ? basePath.Substring(0, pos) : basePath;

            // Get the rest of the url without the domain
            var path = pos > 0 ? basePath.Substring(pos) : string.Empty;

            var newPath = BuildNewPath(path, relativePath);

            // Return the new path or just the root if newPath was empty
            return newPath.Any() ? string.Format("{0}/{1}", root, string.Join("/", newPath)) : root;
        }

        /// <summary>
        /// Resolve any of the supported paths automatically
        /// ResolvePath detects the path type and then resolves the relative path
        /// </summary>
        /// <param name="basePath">Intial source or starting path</param>
        /// <param name="relativePath">Relative path pointing to target</param>
        /// <returns>A fully qualified path to target based on the resolved relative path</returns>
        public string ResolvePath(string basePath, string relativePath)
        {
            return pathResolver[GetPathStyle(basePath)](basePath, relativePath);
        }

        /// <summary>
        /// Splits up both the initial base path minus the root along with the relative path
        /// and then recombines the 2 returning a list of path segments
        /// </summary>
        /// <param name="path">Initial base/source path</param>
        /// <param name="relativePath">Relative target path</param>
        /// <returns>List of path segments that make up the new path</returns>
        private static IList<string> BuildNewPath(string path, string relativePath)
        {
            // Split up the base path and the relative path into a list of
            // path segments
            var basePathSegments = SplitPath(path);
            var relativePathSegments = SplitPath(relativePath);

            foreach (var relativeSegment in relativePathSegments)
            {
                if (relativeSegment == "..")
                {
                    // Remove a path segment from the base path
                    if (basePathSegments.Count == 0)
                    {
                        throw new Exception(RelativePathOutOfBounds);
                    }

                    basePathSegments.Remove(basePathSegments[basePathSegments.Count - 1]);
                }
                else
                {
                    basePathSegments.Add(relativeSegment);
                }
            }

            return basePathSegments;
        }

        /// <summary>
        /// Detects the type of path being resolved e.g. Windows, Url...
        /// </summary>
        /// <param name="basePath">Intitial source path</param>
        /// <returns>Enum value indicating the format of the path</returns>
        private static PathStyle GetPathStyle(string basePath)
        {
            // initial a list of functions to test the path style
            var lookUp = new List<KeyValuePair<Func<string, bool>, PathStyle>>
                             {
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.Contains(@":\"), PathStyle.Directory),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.StartsWith(@"\\"), PathStyle.Unc),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.Contains("//"), PathStyle.Url),
                                 new KeyValuePair<Func<string, bool>, PathStyle>(x => x.StartsWith("/"), PathStyle.Linux)
                             };

            // Find the pathstyle associated with the first condition that passess
            var pathStyle = lookUp.Where(x => x.Key(basePath)).Select(x => x.Value).FirstOrDefault();

            // If not pathstyle if found then throw an exception
            if (0 == pathStyle)
            {
                throw new Exception(InvalidPathStyleExceptionMessage);
            }

            return pathStyle;
        }

        /// <summary>
        /// Splits up a path minus the root into a list of path segements
        /// The format of the path does not matter because both slash types
        /// are detected.
        /// </summary>
        /// <param name="path">Path being split up</param>
        /// <returns>List of Path segments</returns>
        private static List<string> SplitPath(string path)
        {
            var parts =
                path.Split(@"\/".ToCharArray()).Where(part => !string.IsNullOrWhiteSpace(part.Trim())).ToList();
            return parts;
        }
    }
}