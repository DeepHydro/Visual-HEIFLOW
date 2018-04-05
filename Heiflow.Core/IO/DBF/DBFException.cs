// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.IO;

namespace DotNetDBF
{
    public class DBFException : IOException
    {
        public DBFException() : base()
        {
        }

        public DBFException(String msg) : base(msg)
        {
        }

        public DBFException(String msg, Exception internalException)
            : base(msg, internalException)
        {
        }
    }
}