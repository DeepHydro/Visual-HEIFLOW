// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Collections.Generic;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents cell-indexing record, finishes each row values block
	/// </summary>
	internal class XlsBiffDbCell : XlsBiffRecord
	{
		internal XlsBiffDbCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Offset of first row linked with this record
		/// </summary>
		public int RowAddress
		{
			get { return (Offset - base.ReadInt32(0x0)); }
		}

		/// <summary>
		/// Addresses of cell values
		/// </summary>
		public uint[] CellAddresses
		{
			get
			{
				int a = RowAddress - 20; // 20 assumed to be row structure size

				List<uint> tmp = new List<uint>();
				for (int i = 0x4; i < RecordSize; i += 4)
					tmp.Add((uint) a + base.ReadUInt16(i));
				return tmp.ToArray();
			}
		}
	}
}