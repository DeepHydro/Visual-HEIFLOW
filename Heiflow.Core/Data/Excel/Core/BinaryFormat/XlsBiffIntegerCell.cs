// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a constant integer number in range 0..65535
	/// </summary>
	internal class XlsBiffIntegerCell : XlsBiffBlankCell
	{
		internal XlsBiffIntegerCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Returns value of this cell
		/// </summary>
		public uint Value
		{
			get { return base.ReadUInt16(0x6); }
		}
	}
}