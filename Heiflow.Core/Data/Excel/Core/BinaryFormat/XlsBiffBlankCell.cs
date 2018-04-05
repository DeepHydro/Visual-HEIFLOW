// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents blank cell
	/// Base class for all cell types
	/// </summary>
	internal class XlsBiffBlankCell : XlsBiffRecord
	{
		internal XlsBiffBlankCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Zero-based index of row containing this cell
		/// </summary>
		public ushort RowIndex
		{
			get { return base.ReadUInt16(0x0); }
		}

		/// <summary>
		/// Zero-based index of column containing this cell
		/// </summary>
		public ushort ColumnIndex
		{
			get { return base.ReadUInt16(0x2); }
		}

		/// <summary>
		/// Format used for this cell
		/// </summary>
		public ushort XFormat
		{
			get { return base.ReadUInt16(0x4); }
		}
	}
}