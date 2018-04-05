// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Text;

namespace DotNetDBF
{
    abstract public class DBFBase
    {
        protected Encoding _charEncoding = Encoding.GetEncoding("utf-8");
        protected int _blockSize = 512;

        public Encoding CharEncoding
        {
            set { _charEncoding = value; }

            get { return _charEncoding; }
        }
        public int BlockSize
        {
            set { _blockSize = value; }

            get { return _blockSize; }
        }
    }
}