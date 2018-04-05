// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents record with the only two-bytes value
	/// </summary>
	internal class XlsBiffSimpleValueRecord : XlsBiffRecord
	{
		internal XlsBiffSimpleValueRecord(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Returns value
		/// </summary>
		public ushort Value
		{
			get { return ReadUInt16(0x0); }
		}
	}
}
