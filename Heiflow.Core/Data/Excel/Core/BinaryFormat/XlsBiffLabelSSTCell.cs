// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a string stored in SST
	/// </summary>
	internal class XlsBiffLabelSSTCell : XlsBiffBlankCell
	{
		internal XlsBiffLabelSSTCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Index of string in Shared String Table
		/// </summary>
		public uint SSTIndex
		{
			get { return base.ReadUInt32(0x6); }
		}

		/// <summary>
		/// Returns text using specified SST
		/// </summary>
		/// <param name="sst">Shared String Table record</param>
		/// <returns></returns>
		public string Text(XlsBiffSST sst)
		{
			return sst.GetString(SSTIndex);
		}
	}
}