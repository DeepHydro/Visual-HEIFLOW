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

using System.Data;

namespace DotNetDBF
{
    public enum NativeDbType :byte
    {
        Autoincrement = (byte) 0x2B, //+ in ASCII
        Timestamp = (byte) 0x40, //@ in ASCII
        Binary = (byte) 0x42, //B in ASCII
        Char = (byte) 0x43, //C in ASCII
        Date = (byte) 0x44, //D in ASCII
        Float = (byte) 0x46, //F in ASCII
        Ole = (byte) 0x47, //G in ASCII
        Long = (byte) 0x49, //I in ASCII
        Logical = (byte) 0x4C, //L in ASCII
        Memo = (byte) 0x4D, //M in ASCII
        Numeric = (byte) 0x4E, //N in ASCII
        Double = (byte) 0x4F, //O in ASCII
    }

    static public class DBFFieldType
    {
        public const byte EndOfData = 0x1A; //^Z End of File
        public const byte EndOfField = 0x0D; //End of Field
        public const byte False = 0x46; //F in Ascci
        public const byte Space = 0x20; //Space in ascii
        public const byte True = 0x54; //T in ascii
        public const byte UnknownByte = 0x3F; //Unknown Bool value
        public const string Unknown = "?"; //Unknown value
        static public DbType FromNative(NativeDbType aByte)
        {
            switch (aByte)
            {
                case NativeDbType.Char:
                    return DbType.AnsiStringFixedLength;
                case NativeDbType.Logical:
                    return DbType.Boolean;
                case NativeDbType.Numeric:
                    return DbType.Decimal;
                case NativeDbType.Date:
                    return DbType.Date;
                case NativeDbType.Float:
                    return DbType.Decimal;
                case NativeDbType.Memo:
                    return DbType.AnsiString;
                default:
                    throw new DBFException(
                        string.Format("Unsupported Native Type {0}", aByte));
            }
        }

        static public NativeDbType FromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiStringFixedLength:
                    return NativeDbType.Char;
                case DbType.Boolean:
                    return NativeDbType.Logical;
                case DbType.Decimal:
                    return NativeDbType.Numeric;
                case DbType.Date:
                    return NativeDbType.Date;
                case DbType.AnsiString:
                    return NativeDbType.Memo;
                default:
                    throw new DBFException(
                        string.Format("Unsupported Type {0}", dbType));
            }
        }
    }
}