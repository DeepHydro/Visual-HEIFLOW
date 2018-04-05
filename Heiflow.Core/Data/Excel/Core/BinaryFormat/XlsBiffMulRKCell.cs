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

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents multiple RK number cells
	/// </summary>
	internal class XlsBiffMulRKCell : XlsBiffBlankCell
	{
		internal XlsBiffMulRKCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Returns zero-based index of last described column
		/// </summary>
		public ushort LastColumnIndex
		{
			get { return base.ReadUInt16(RecordSize - 2); }
		}

		/// <summary>
		/// Returns format for specified column
		/// </summary>
		/// <param name="ColumnIdx">Index of column, must be between ColumnIndex and LastColumnIndex</param>
		/// <returns></returns>
		public ushort GetXF(ushort ColumnIdx)
		{
			int ofs = 4 + 6*(ColumnIdx - ColumnIndex);
			if (ofs > RecordSize - 2)
				return 0;
			return base.ReadUInt16(ofs);
		}

		/// <summary>
		/// Returns value for specified column
		/// </summary>
		/// <param name="ColumnIdx">Index of column, must be between ColumnIndex and LastColumnIndex</param>
		/// <returns></returns>
		public double GetValue(ushort ColumnIdx)
		{
			int ofs = 6 + 6*(ColumnIdx - ColumnIndex);
			if (ofs > RecordSize)
				return 0;
			return XlsBiffRKCell.NumFromRK(base.ReadUInt32(ofs));
		}
	}
}