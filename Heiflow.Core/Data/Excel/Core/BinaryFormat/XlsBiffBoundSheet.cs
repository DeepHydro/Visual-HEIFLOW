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

using System.Text;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents Sheet record in Workbook Globals
	/// </summary>
	internal class XlsBiffBoundSheet : XlsBiffRecord
	{
		#region SheetType enum

		public enum SheetType : byte
		{
			Worksheet = 0x0,
			MacroSheet = 0x1,
			Chart = 0x2,
			VBModule = 0x6
		}

		#endregion

		#region SheetVisibility enum

		public enum SheetVisibility : byte
		{
			Visible = 0x0,
			Hidden = 0x1,
			VeryHidden = 0x2
		}

		#endregion

		private bool isV8 = true;
		private Encoding m_UseEncoding = Encoding.Default;

		internal XlsBiffBoundSheet(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}
		/// <summary>
		/// Worksheet data start offset
		/// </summary>
		public uint StartOffset
		{
			get { return base.ReadUInt32(0x0); }
		}

		/// <summary>
		/// Type of worksheet
		/// </summary>
		public SheetType Type
		{
			get { return (SheetType)base.ReadByte(0x4); }
		}

		/// <summary>
		/// Visibility of worksheet
		/// </summary>
		public SheetVisibility VisibleState
		{
			get { return (SheetVisibility)(base.ReadByte(0x5) & 0x3); }
		}

		/// <summary>
		/// Name of worksheet
		/// </summary>
		public string SheetName
		{
			get
			{
				ushort len = base.ReadByte(0x6);

				int start = 0x8;
				if (isV8)
					if (base.ReadByte(0x7) == 0)
						return Encoding.Default.GetString(m_bytes, m_readoffset + start, len);
					else
						return m_UseEncoding.GetString(m_bytes, m_readoffset + start, Helpers.IsSingleByteEncoding(m_UseEncoding) ? len : len * 2);
				else
					return Encoding.Default.GetString(m_bytes, m_readoffset + start - 1, len);
			}
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
		/// Specifies if BIFF8 format should be used
		/// </summary>
		public bool IsV8
		{
			get { return isV8; }
			set { isV8 = value; }
		}
	}
}