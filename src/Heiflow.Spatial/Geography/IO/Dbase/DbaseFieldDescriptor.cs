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
 

namespace Heiflow.Spatial.Geography.IO
{
    /// <summary>
    /// Class for holding the information assicated with a dbase field.
    /// </summary>
    public class DbaseFieldDescriptor
    {
        // Field Name
        private string _name;

        // Field Type (C N L D or M)
        private char _type;

        // Field Data Address offset from the start of the record.
        private int _dataAddress;

        // Length of the data in bytes
        private int _length;

        // Field decimal count in Binary, indicating where the decimal is
        private int _decimalCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static char GetDbaseType(Type type)
        {
            DbaseFieldDescriptor dbaseColumn = new DbaseFieldDescriptor();
            if (type == typeof(Char))
                return 'C';
            if (type == typeof(string))
                return 'C';
            else if (type == typeof(Double))
                return 'N';
            else if (type == typeof(Single))
                return 'N';
            else if (type == typeof(Int16))
                return 'N';
            else if (type == typeof(Int32))
                return 'N';
            else if (type == typeof(Int64))
                return 'N';
            else if (type == typeof(UInt16))
                return 'N';
            else if (type == typeof(UInt32))
                return 'N';
            else if (type == typeof(UInt64))
                return 'N';
            else if (type == typeof(Decimal))
                return 'N';
            else if (type == typeof(Boolean))
                return 'L';
            else if (type == typeof(DateTime))
                return 'D';

            throw new NotSupportedException(String.Format("{0} does not have a corresponding dbase type.", type.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DbaseFieldDescriptor ShapeField()
        {
            DbaseFieldDescriptor shpfield = new DbaseFieldDescriptor();
            shpfield.Name = "Geometry";
            shpfield._type = 'B';
            return shpfield;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DbaseFieldDescriptor IdField()
        {
            DbaseFieldDescriptor shpfield = new DbaseFieldDescriptor();
            shpfield.Name = "Row";
            shpfield._type = 'I';
            return shpfield;
        }

        /// <summary>
        /// Field Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Field Type (C N L D or M).
        /// </summary>
        public char DbaseType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Field Data Address offset from the start of the record.
        /// </summary>
        public int DataAddress
        {
            get
            {
                return _dataAddress;
            }
            set
            {
                _dataAddress = value;
            }
        }

        /// <summary>
        /// Length of the data in bytes.
        /// </summary>
        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        /// <summary>
        /// Field decimal count in Binary, indicating where the decimal is.
        /// </summary>
        public int DecimalCount
        {
            get
            {
                return _decimalCount;
            }
            set
            {
                _decimalCount = value;
            }
        }

        /// <summary>
        /// Returns the equivalent CLR type for this field.
        /// </summary>
        public Type Type
        {
            get
            {
                Type type;
                switch (_type)
                {
                    case 'L': // logical data type, one character (T,t,F,f,Y,y,N,n)
                        type = typeof(bool);
                        break;
                    case 'C': // char or string
                        type = typeof(string);
                        break;
                    case 'D': // date
                        type = typeof(DateTime);
                        break;
                    case 'N': // numeric
                        type = typeof(double);
                        break;
                    case 'F': // double
                        type = typeof(float);
                        break;
                    case 'B': // BLOB - not a dbase but this will hold the WKB for a geometry object.
                        type = typeof(byte[]);
                        break;
                    default:
                        throw new NotSupportedException("Do not know how to parse Field type " + _type);
                }
                return type;
            }
        }
    }
}
