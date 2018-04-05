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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DotNetDBF
{
    static public class Utils
    {

        public const int ALIGN_LEFT = 10;
        public const int ALIGN_RIGHT = 12;

        static public byte[] FillArray(byte[] anArray, byte value)
        {
            for (int i = 0; i < anArray.Length; i++)
            {
                anArray[i] = value;
            }
            return anArray;
        }

        static public byte[] trimLeftSpaces(byte[] arr)
        {
            List<byte> tList = new List<byte>(arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != ' ')
                {
                    tList.Add(arr[i]);
                }
            }
            return tList.ToArray();
        }

        static public byte[] textPadding(String text,
                                         Encoding charEncoding,
                                         int length)
        {
            return textPadding(text, charEncoding, length, ALIGN_LEFT);
        }

        static public byte[] textPadding(String text,
                                         Encoding charEncoding,
                                         int length,
                                         int alignment)
        {
            return
                textPadding(text,
                            charEncoding,
                            length,
                            alignment,
                            DBFFieldType.Space);
        }

        static public byte[] textPadding(String text,
                                         Encoding charEncoding,
                                         int length,
                                         int alignment,
                                         byte paddingByte)
        {
            Encoding tEncoding = charEncoding;
            var inputBytes = tEncoding.GetBytes(text);
            if (inputBytes.Length >= length)
            {
                return inputBytes.Take(length).ToArray();
            }

            byte[] byte_array = FillArray(new byte[length], paddingByte);

            switch (alignment)
            {
                case ALIGN_LEFT:
                    Array.Copy(inputBytes,
                               0,
                               byte_array,
                               0,
                               inputBytes.Length);
                    break;

                case ALIGN_RIGHT:
                    int t_offset = length - text.Length;
                    Array.Copy(inputBytes,
                               0,
                               byte_array,
                               t_offset,
                               inputBytes.Length);
                    break;
            }

            return byte_array;
        }

        static public byte[] NumericFormating(IFormattable doubleNum,
                                              Encoding charEncoding,
                                              int fieldLength,
                                              int sizeDecimalPart)
        {
            int sizeWholePart = fieldLength
                                -
                                (sizeDecimalPart > 0 ? (sizeDecimalPart + 1) : 0);

            StringBuilder format = new StringBuilder(fieldLength);

            for (int i = 0; i < sizeWholePart; i++)
            {

                format.Append(i+1== sizeWholePart ? "0":"#");
            }

            if (sizeDecimalPart > 0)
            {
                format.Append(".");

                for (int i = 0; i < sizeDecimalPart; i++)
                {
                    format.Append("0");
                }
            }


            return
                textPadding(
                    doubleNum.ToString(format.ToString(),
                                       NumberFormatInfo.InvariantInfo),
                    charEncoding,
                    fieldLength,
                    ALIGN_RIGHT);
        }

        static public bool contains(byte[] arr, byte value)
        {
            return
                Array.Exists(arr,
                             delegate(byte anItem) { return anItem == value; });
        }


        static public Type TypeForNativeDBType(NativeDbType aType)
        {
            switch(aType)
            {
                case NativeDbType.Char:
                    return typeof (string);
                case NativeDbType.Date:
                    return typeof (DateTime);
                case NativeDbType.Numeric:
                    return typeof (decimal);
                case NativeDbType.Logical:
                    return typeof (bool);
                case NativeDbType.Float:
                    return typeof (float);
                case NativeDbType.Memo:
                    return typeof (MemoValue);
                default:
                    throw new ArgumentException("Unsupported Type");
            }
        }
    }
}