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
    public static class StreamReaderSequence
    {
        public static char[] ctab = new char[] { '\t' };
        /// <summary>
        /// ,
        /// </summary>
        public static char[] cCommet = new char[] { ',' };
        public static char[] cEnter = new char[] { '\n' };
        public static string stab = "\t";
        public static string sCommet = ",";
        public static string header = "=======";
        public static Encoding GBEncoding = Encoding.GetEncoding("GB2312");
        public static string Comment = "#";

        public static IEnumerable<string> Lines(this StreamReader source)
        {
            String line;

            if (source == null)
                throw new ArgumentNullException("source");
            while ((line = source.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static double [] ToFloatArray(string line)
        {
            return null;
        }

    }
}
