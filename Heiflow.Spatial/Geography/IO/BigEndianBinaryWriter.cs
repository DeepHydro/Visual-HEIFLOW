// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.IO;
using System.Text;


namespace Heiflow.Spatial.Geography.IO
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the BigEndianBinaryWriter class.
        /// </summary>
        public BigEndianBinaryWriter() : base() { }

        /// <summary>
        /// Initializes a new instance of the BigEndianBinaryWriter class 
        /// based on the supplied stream and using UTF-8 as the encoding for strings.
        /// </summary>
        /// <param name="output">The supplied stream.</param>
        public BigEndianBinaryWriter(Stream output) : base(output) { }

        /// <summary>
        /// Initializes a new instance of the BigEndianBinaryWriter class 
        /// based on the supplied stream and a specific character encoding.
        /// </summary>
        /// <param name="output">The supplied stream.</param>
        /// <param name="encoding">The character encoding.</param>
        public BigEndianBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }

        /// <summary>
        /// Reads a 4-byte signed integer using the big-endian layout from the current stream 
        /// and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public void WriteIntBE(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes, 0, 4);
            Write(bytes);
        }

        /// <summary>
        /// Reads a 8-byte signed integer using the big-endian layout from the current stream 
        /// and advances the current position of the stream by two bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public void WriteDoubleBE(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            Array.Reverse(bytes, 0, 8);
            Write(bytes);
        }
    }
}
