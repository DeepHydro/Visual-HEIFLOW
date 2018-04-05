// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Heiflow.Core.IO
{
    public class FileHelper
    {
        private static string numberPattern = " ({0})";
        public static List<string> SafeNameList = new List<string>();

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public static string GetSafeName(string name, int count)
        {
            if (SafeNameList.Contains(name))
            {
                count += 1;
                return GetSafeName(name + count, count);
            } 
            else
            {
                SafeNameList.Add(name);
                return name;
            }
        }

       public static  string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

       public static string GetRelativeFileName(string masterdic, string fullFileName)
       {
           string folder = Path.GetDirectoryName(masterdic);
           var root_ly = Path.GetPathRoot(fullFileName);
           string relativeFileName = "";
           if (Path.GetPathRoot(fullFileName) == Path.GetPathRoot(fullFileName))
           {
               relativeFileName = FileHelper.GetRelativePath(fullFileName, folder);
           }
           else
           {
               relativeFileName = fullFileName;
           }
           return relativeFileName;
       }
    }
}
