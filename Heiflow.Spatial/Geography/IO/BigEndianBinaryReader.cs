// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Heiflow.Spatial.Geography;

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Extends the <see cref="System.IO.BinaryReader" /> class to allow reading of integers and doubles 
    /// in the Big Endian format.
    /// </summary>
    /// <remarks>
    /// The BinaryReader uses Little Endian format when reading binary streams.
    /// </remarks>
    public class BigEndianBinaryReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the BigEndianBinaryReader class 
        /// based on the supplied stream and using UTF8Encoding.
        /// </summary>
        /// <param name="stream"></param>
        public BigEndianBinaryReader(Stream stream) : base(stream) { }

        /// <summary>
        /// Initializes a new instance of the BigEndianBinaryReader class 
        /// based on the supplied stream and a specific character encoding.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

        /// <summary>
        /// Reads a 4-byte signed integer using the big-endian layout 
        /// from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns></returns>
        public int ReadInt32BE()
        {
            // big endian
            byte[] byteArray = new byte[4];
            int iBytesRead = Read(byteArray, 0, 4);
          

            Array.Reverse(byteArray);
            return BitConverter.ToInt32(byteArray, 0);
        }

        /// <summary>
        /// Reads a 8-byte signed double using the big-endian layout 
        /// from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns></returns>        
        public double ReadDoubleBE()
        {
            // big endian
            byte[] byteArray = new byte[8];
            int iBytesRead = Read(byteArray, 0, 8);

            Array.Reverse(byteArray);
            return BitConverter.ToDouble(byteArray, 0);
        }
    }
   
}
