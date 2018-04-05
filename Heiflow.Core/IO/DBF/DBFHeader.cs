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
using System.IO;

namespace DotNetDBF
{

    static public class DBFSigniture
    {
        public const byte NotSet = 0,
                          WithMemo = 0x80,
                          DBase3 = 0x03,
                          DBase3WithMemo = DBase3 | WithMemo;

    }

    [Flags]
    public enum MemoFlags : byte
    {
        
    }


    public class DBFHeader
    {

        public const byte HeaderRecordTerminator = 0x0D;

        private byte _day; /* 3 */
        private byte _encryptionFlag; /* 15 */
        private DBFField[] _fieldArray; /* each 32 bytes */
        private int _freeRecordThread; /* 16-19 */
        private short _headerLength; /* 8-9 */
        private byte _incompleteTransaction; /* 14 */
        private byte _languageDriver; /* 29 */
        private byte _mdxFlag; /* 28 */
        private byte _month; /* 2 */
        private int _numberOfRecords; /* 4-7 */
        private short _recordLength; /* 10-11 */
        private short _reserv1; /* 12-13 */
        private int _reserv2; /* 20-23 */
        private int _reserv3; /* 24-27 */
        private short reserv4; /* 30-31 */
        private byte _signature; /* 0 */
        private byte _year; /* 1 */

        public DBFHeader()
        {
            _signature = DBFSigniture.DBase3;
        }

        internal byte Signature
        {
            get
            {
                return _signature;
            }set
            {
                _signature = value;
            }
        }

        internal short Size
        {
            get
            {
                return (short) (sizeof (byte) +
                                sizeof (byte) + sizeof (byte) + sizeof (byte) +
                                sizeof (int) +
                                sizeof (short) +
                                sizeof (short) +
                                sizeof (short) +
                                sizeof (byte) +
                                sizeof (byte) +
                                sizeof (int) +
                                sizeof (int) +
                                sizeof (int) +
                                sizeof (byte) +
                                sizeof (byte) +
                                sizeof (short) +
                                (DBFField.SIZE * _fieldArray.Length) +
                                sizeof (byte));
            }
        }

        internal short RecordSize
        {
            get
            {
                int tRecordLength = 0;
                for (int i = 0; i < _fieldArray.Length; i++)
                {
                    tRecordLength += _fieldArray[i].FieldLength;
                }

                return (short)(tRecordLength + 1);
            }
        }

        internal short HeaderLength
        {
            set { _headerLength = value; }

            get { return _headerLength; }
        }

        internal DBFField[] FieldArray
        {
            set { _fieldArray = value; }

            get { return _fieldArray; }
        }

        internal byte Year
        {
            set { _year = value; }

            get { return _year; }
        }

        internal byte Month
        {
            set { _month = value; }

            get { return _month; }
        }

        internal byte Day
        {
            set { _day = value; }

            get { return _day; }
        }

        internal int NumberOfRecords
        {
            set { _numberOfRecords = value; }

            get { return _numberOfRecords; }
        }

        internal short RecordLength
        {
            set { _recordLength = value; }

            get { return _recordLength; }
        }

        internal byte LanguageDriver
        {
            get { return _languageDriver; }
            set { _languageDriver = value; }
        }

        internal void Read(BinaryReader dataInput)
        {
            _signature = dataInput.ReadByte(); /* 0 */
            _year = dataInput.ReadByte(); /* 1 */
            _month = dataInput.ReadByte(); /* 2 */
            _day = dataInput.ReadByte(); /* 3 */
            _numberOfRecords = dataInput.ReadInt32(); /* 4-7 */

            _headerLength = dataInput.ReadInt16(); /* 8-9 */
            _recordLength = dataInput.ReadInt16(); /* 10-11 */

            _reserv1 = dataInput.ReadInt16(); /* 12-13 */
            _incompleteTransaction = dataInput.ReadByte(); /* 14 */
            _encryptionFlag = dataInput.ReadByte(); /* 15 */
            _freeRecordThread = dataInput.ReadInt32(); /* 16-19 */
            _reserv2 = dataInput.ReadInt32(); /* 20-23 */
            _reserv3 = dataInput.ReadInt32(); /* 24-27 */
            _mdxFlag = dataInput.ReadByte(); /* 28 */
            _languageDriver = dataInput.ReadByte(); /* 29 */
            reserv4 = dataInput.ReadInt16(); /* 30-31 */


            List<DBFField> v_fields = new List<DBFField>();

            DBFField field = DBFField.CreateField(dataInput); /* 32 each */
            while (field != null)
            {
                v_fields.Add(field);
                field = DBFField.CreateField(dataInput);
            }

            _fieldArray = v_fields.ToArray();
            //System.out.println( "Number of fields: " + _fieldArray.length);
        }

        internal void Write(BinaryWriter dataOutput)
        {
            dataOutput.Write(_signature); /* 0 */
            DateTime tNow = DateTime.Now;
            _year = (byte) (tNow.Year - 1900);
            _month = (byte) (tNow.Month);
            _day = (byte) (tNow.Day);

            dataOutput.Write(_year); /* 1 */
            dataOutput.Write(_month); /* 2 */
            dataOutput.Write(_day); /* 3 */

            //System.out.println( "Number of records in O/S: " + numberOfRecords);
            dataOutput.Write(_numberOfRecords); /* 4-7 */

            _headerLength = Size;
            dataOutput.Write(_headerLength); /* 8-9 */

            _recordLength = RecordSize;
            dataOutput.Write(_recordLength); /* 10-11 */

            dataOutput.Write(_reserv1); /* 12-13 */
            dataOutput.Write(_incompleteTransaction); /* 14 */
            dataOutput.Write(_encryptionFlag); /* 15 */
            dataOutput.Write(_freeRecordThread); /* 16-19 */
            dataOutput.Write(_reserv2); /* 20-23 */
            dataOutput.Write(_reserv3); /* 24-27 */

            dataOutput.Write(_mdxFlag); /* 28 */
            dataOutput.Write(_languageDriver); /* 29 */
            dataOutput.Write(reserv4); /* 30-31 */

            for (int i = 0; i < _fieldArray.Length; i++)
            {
                //System.out.println( "Length: " + _fieldArray[i].getFieldLength());
                _fieldArray[i].Write(dataOutput);
            }

            dataOutput.Write(HeaderRecordTerminator); /* n+1 */
        }
    }
}