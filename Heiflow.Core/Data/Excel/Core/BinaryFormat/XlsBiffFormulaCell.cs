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
using System.Text;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a cell containing formula
	/// </summary>
	internal class XlsBiffFormulaCell : XlsBiffNumberCell
	{
		#region FormulaFlags enum

		[Flags]
		public enum FormulaFlags : ushort
		{
			AlwaysCalc = 0x0001,
			CalcOnLoad = 0x0002,
			SharedFormulaGroup = 0x0008
		}

		#endregion

		private Encoding m_UseEncoding = Encoding.Default;

		internal XlsBiffFormulaCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
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
		/// Formula flags
		/// </summary>
		public FormulaFlags Flags
		{
			get { return (FormulaFlags)(base.ReadUInt16(0xE)); }
		}

		/// <summary>
		/// Length of formula string
		/// </summary>
		public byte FormulaLength
		{
			get { return base.ReadByte(0xF); }
		}

		/// <summary>
		/// Returns type-dependent value of formula
		/// </summary>
		public new object Value
		{
			get
			{
				long val = base.ReadInt64(0x6);
				if (((ulong)val & 0xFFFF000000000000) == 0xFFFF000000000000)
				{
					byte type = (byte)(val & 0xFF);
					byte code = (byte)((val >> 16) & 0xFF);
					switch (type)
					{
						case 0: // String

                            //////////////fix
                            XlsBiffRecord rec = GetRecord(m_bytes, (uint)(Offset + Size), reader);
                            XlsBiffFormulaString str;
                            if (rec.ID == BIFFRECORDTYPE.SHRFMLA)
								str = GetRecord(m_bytes, (uint)(Offset + Size + rec.Size), reader) as XlsBiffFormulaString;
                            else
                                str = rec as XlsBiffFormulaString;
                            //////////////fix

                            if (str == null)
                                return string.Empty;
                            else
                            {
                                str.UseEncoding = m_UseEncoding;
                                return str.Value;
                            }
						case 1: // Boolean

							return (code != 0);
						case 2: // Error

							return (FORMULAERROR)code;
						default:
							return null;
					}
				}
				else
					return Helpers.Int64BitsToDouble(val);
			}
		}

		public string Formula
		{
			get
			{
				byte[] bts = base.ReadArray(0x10, FormulaLength);
				return Encoding.Default.GetString(bts, 0, bts.Length);
			}
		}
	}
}