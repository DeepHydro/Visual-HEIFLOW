//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Utility
{
    public class DirectoryHelper
    {
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        public static bool Compare(string path1, string path2)
        {
            if(TypeConverterEx.IsNull(path1) ||TypeConverterEx.IsNull(path2))
            {
                return false;
            }
            else
            {
            bool pathsEqual = NormalizePath(path1) == NormalizePath(path2);
            return pathsEqual;
            }
        }

        /// <summary>
        /// Creates a relative path from one file
        /// or folder to another.
        /// </summary>
        /// <param name="toPath">
        /// Contains the path that defines the
        /// endpoint of the relative path.
        /// </param>
        /// <returns>
        /// The relative path from the start
        /// directory to the end path.
        /// </returns>
        /// <exception cref="ArgumentNullException">Occurs when the toPath is NULL</exception>
        //http://weblogs.asp.net/pwelter34/archive/2006/02/08/create-a-relative-path-code-snippet.aspx
        public static string RelativePathTo(string toPath, string fromDirectory)
        {
            if (toPath == null)
                throw new ArgumentNullException("toPath");

            bool isRooted = Path.IsPathRooted(fromDirectory)
                            && Path.IsPathRooted(toPath);

            if (isRooted)
            {
                bool isDifferentRoot = string.Compare(
                    Path.GetPathRoot(fromDirectory),
                    Path.GetPathRoot(toPath), true) != 0;

                if (isDifferentRoot)
                    return toPath;
            }

            StringCollection relativePath = new StringCollection();
            string[] fromDirectories = fromDirectory.Split(
                Path.DirectorySeparatorChar);

            string[] toDirectories = toPath.Split(
                Path.DirectorySeparatorChar);

            int length = Math.Min(
                fromDirectories.Length,
                toDirectories.Length);

            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (string.Compare(fromDirectories[x],
                                   toDirectories[x], true) != 0)
                    break;

                lastCommonRoot = x;
            }
            if (lastCommonRoot == -1)
                return toPath;

            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
                if (fromDirectories[x].Length > 0)
                    relativePath.Add("..");

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
                relativePath.Add(toDirectories[x]);

            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);

            string newPath = string.Join(
                Path.DirectorySeparatorChar.ToString(),
                relativeParts);

            return newPath;
        }

        public static string RelativePathTo(string toPath)
        {
            var dic = Directory.GetCurrentDirectory();
            return RelativePathTo(toPath, dic);
        }

        public static void Create(string dic)
        {
            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (String.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            if (Path.GetDirectoryName(toPath) == String.Empty)
                // it looks like we only have a file name.
                return toPath;

            if (!fromPath.EndsWith(@"\"))
                fromPath += @"\";

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);

            return relativeUri.ToString();
        }

        public static string AbsolutePathTo(string toPath)
        {
            if (String.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            return Path.GetFullPath(toPath);
        }
        public static string AbsolutePathTo(string toPath, string basePath)
        {
            if (String.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            return Path.Combine(basePath, toPath);
        }

        public static bool IsRelativePath(string path)
        {
            if (TypeConverterEx.IsNotNull(path))
            {
                if (path.StartsWith(@".\") || path.StartsWith(@"..\") || !Path.IsPathRooted(path))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static string GetUniqueString()
        {
            //Guid g = Guid.NewGuid();
            //string GuidString = Convert.ToBase64String(g.ToByteArray());
            //GuidString = GuidString.Replace("=", "");
            //GuidString = GuidString.Replace("+", "");
            // return GuidString;
            var str = Path.GetRandomFileName();
            str = str.Replace(".", "x");
            return str;
        }
    }
}