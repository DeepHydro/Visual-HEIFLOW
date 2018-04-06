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

using System.Text;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a string value of formula
	/// </summary>
	internal class XlsBiffFormatString : XlsBiffRecord
	{

        private Encoding m_UseEncoding =  Encoding.Default;
		private string m_value = null;
		
        internal XlsBiffFormatString(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}


        /// <summary>
        /// Encoding used to deal with strings
        /// </summary>
        public Encoding UseEncoding
        {
            get { return m_UseEncoding; }
            set { m_UseEncoding = value; }
        }

		/// <summary>
		/// Length of the string
		/// </summary>
		public ushort Length
		{
			get
			{
			     switch (ID)
			     {
			         case BIFFRECORDTYPE.FORMAT_V23:
			             return base.ReadByte(0x0);
			         default:
			             return base.ReadUInt16(2);
			     }
			}
		}

		/// <summary>
		/// String text
		/// </summary>
        public string Value
        {
            get
            {
                if (m_value == null)
                {
                    switch (ID)
                    {
                        case BIFFRECORDTYPE.FORMAT_V23:
                            m_value = m_UseEncoding.GetString(m_bytes, m_readoffset + 1, Length);
                            break;
                        case BIFFRECORDTYPE.FORMAT:
                            var offset = m_readoffset + 5;
                            var flags = ReadByte(3);
                            m_UseEncoding = (flags & 0x01) == 0x01 ? Encoding.Unicode : Encoding.Default;
                            if ((flags & 0x04) == 0x01) // asian phonetic block size
                                offset += 4;
                            if ((flags & 0x08) == 0x01) // number of rtf blocks
                                offset += 2;
                            m_value = m_UseEncoding.IsSingleByte ? m_UseEncoding.GetString(m_bytes, offset, Length) : m_UseEncoding.GetString(m_bytes, offset, Length*2);

                            break;


                    }
                   

                }
                return m_value;
            }
        }

        public ushort Index
        {
            get
            {
                switch (ID)
                {
                    case BIFFRECORDTYPE.FORMAT_V23:
                        return 0;
                    default:
                        return ReadUInt16(0);

                }
            }
        }
	}
}