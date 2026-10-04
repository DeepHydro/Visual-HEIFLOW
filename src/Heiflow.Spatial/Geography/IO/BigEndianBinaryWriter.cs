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
