// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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