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

using System.Collections.Generic;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a worksheet index
	/// </summary>
	internal class XlsBiffIndex : XlsBiffRecord
	{
		private bool isV8 = true;

		internal XlsBiffIndex(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Gets or sets if BIFF8 addressing is used
		/// </summary>
		public bool IsV8
		{
			get { return isV8; }
			set { isV8 = value; }
		}

		/// <summary>
		/// Returns zero-based index of first existing row
		/// </summary>
		public uint FirstExistingRow
		{
			get { return (isV8) ? base.ReadUInt32(0x4) : base.ReadUInt16(0x4); }
		}

		/// <summary>
		/// Returns zero-based index of last existing row
		/// </summary>
		public uint LastExistingRow
		{
			get { return (isV8) ? base.ReadUInt32(0x8) : base.ReadUInt16(0x6); }
		}

		/// <summary>
		/// Returns addresses of DbCell records
		/// </summary>
		public uint[] DbCellAddresses
		{
			get
			{
				int size = RecordSize;
				int firstIdx = (isV8) ? 16 : 12;
				if (size <= firstIdx)
					return new uint[0];
				List<uint> cells = new List<uint>((size - firstIdx)/4);
				for (int i = firstIdx; i < size; i += 4)
					cells.Add(base.ReadUInt32(i));
				return cells.ToArray();
			}
		}
	}
}