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
